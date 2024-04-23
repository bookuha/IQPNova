using FluentValidation;
using IQP.Application.Contracts.AlgoTasks.Commands;
using IQP.Application.Contracts.AlgoTasks.Responses;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.CodeRunner;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Services;

public class AlgoTasksService : IAlgoTasksService
{
    private readonly IqpDbContext _db;
    private readonly ITestRunnerService _codeFileTestRunner;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<RunTestsOnCodeCommand> _runTestsOnCodeCommandValidator;
    private readonly IValidator<SubmitAlgoTaskSolutionCommand> _submitCodeCommandValidator;
    private readonly IValidator<CreateAlgoTaskCommand> _createAlgoTaskCommandValidator;
    private readonly IValidator<UpdateAlgoTaskCommand> _updateAlgoTaskCommandValidator;
    private readonly IValidator<AddNewLanguageToAlgoTaskCommand> _addNewLanguageToAlgoTaskCommandValidator;
    private readonly ILogger<AlgoTasksService> _logger;
    private readonly IUserService _userService;
    

    public AlgoTasksService(
        IqpDbContext db,
        ITestRunnerService codeFileTestRunner,
        ICurrentUserService currentUser,
        IValidator<RunTestsOnCodeCommand> runTestsOnCodeCommandValidator, 
        IValidator<SubmitAlgoTaskSolutionCommand> submitCodeCommandValidator,
        IValidator<CreateAlgoTaskCommand> createAlgoTaskCommandValidator,
        IValidator<UpdateAlgoTaskCommand> updateAlgoTaskCommandValidator,
        IValidator<AddNewLanguageToAlgoTaskCommand> addNewLanguageToAlgoTaskCommandValidator,
        ILogger<AlgoTasksService> logger,
        IUserService userService)
    {
        _db = db;
        _codeFileTestRunner = codeFileTestRunner;
        _currentUser = currentUser;
        _runTestsOnCodeCommandValidator = runTestsOnCodeCommandValidator;
        _submitCodeCommandValidator = submitCodeCommandValidator;
        _createAlgoTaskCommandValidator = createAlgoTaskCommandValidator;
        _updateAlgoTaskCommandValidator = updateAlgoTaskCommandValidator;
        _addNewLanguageToAlgoTaskCommandValidator = addNewLanguageToAlgoTaskCommandValidator;
        _logger = logger;
        _userService = userService;
    }

    private static IEnumerable<CodeLanguage> GetTaskSupportedLanguages(AlgoTask algoTask) => algoTask.CodeSnippets.Select(t => t.Language);

    private async Task<bool> ValidateAlgoTaskIsPassable(string initialSolutionCode, string testsCode, CodeLanguage language)
    {
        var result = await _codeFileTestRunner.RunTestsOnCode(initialSolutionCode, testsCode, language.Slug, Guid.NewGuid().ToString());
        
        return result.Status is TestStatus.Pass;
    }
    
    public async Task<AlgoTaskResponse> CreateAlgoTask(CreateAlgoTaskCommand command)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }

        
        var commandValidationResult = _createAlgoTaskCommandValidator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask, commandValidationResult.ToDictionary()); // Maybe make subentity?
        }
        
        var titleAlreadyExists = await _db.AlgoTasks.AnyAsync(c => c.Title == command.Title);
        
        if (titleAlreadyExists)
        {
            throw new IqpException(
                EntityName.AlgoTask,Errors.AlreadyExists.ToString(), "Already exists", "The algo task with such title already exists.");
        }
        
        var algoCategory = await _db.AlgoTaskCategories.FindAsync(command.AlgoCategoryId);

        if (algoCategory is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoCategory}", Errors.NotFound.ToString(), "AlgoCategory not found",
                "The algo category with such id does not exist. Therefore algo task cannot be created.");
        }
        
        var language = await _db.CodeLanguages.FindAsync(command.InitialCodeSnippet.LanguageId);
        
        if (language is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}.{EntityName.CodeLanguage}", Errors.NotFound.ToString(), "CodeLanguage not found",
                "The code language with such id does not exist or is not supported. Therefore algo task cannot be created.");
        }
        
        var isAlgoTaskPassable = await ValidateAlgoTaskIsPassable(
            command.InitialCodeSnippet.InitialSolutionCode,
            command.InitialCodeSnippet.TestsCode,
            language);

        if (!isAlgoTaskPassable)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.WrongFlow.ToString(), "Not passable",
                "The initial solution does not pass the tests. Therefore algo task cannot be created.");
        }
        
        var algoTask = new AlgoTask
        {
            Title = command.Title,
            Description = command.Description,
            AlgoCategoryId = command.AlgoCategoryId,
            CodeSnippets = new List<AlgoTaskCodeSnippet>
                {
                    new()
                    {
                        Language = language,
                        SampleCode = command.InitialCodeSnippet.SampleCode,
                        TestsCode = command.InitialCodeSnippet.TestsCode
                    }
                },
        };

        await _db.AlgoTasks.AddAsync(algoTask);
        await _db.SaveChangesAsync();

        return algoTask.ToResponse(GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, false);
    }
    
    public async Task<AlgoTaskResponse> AddNewLanguageToAlgoTask(AddNewLanguageToAlgoTaskCommand command)
    {
        var commandValidationResult = _addNewLanguageToAlgoTaskCommandValidator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask, commandValidationResult.ToDictionary()); // Maybe make subentity?
        }
        
        var algoTask = await _db.AlgoTasks
            .Include(t=>t.AlgoCategory)
            .Include(t=>t.CodeSnippets)
                .ThenInclude(c=>c.Language)
            .SingleOrDefaultAsync(t=>t.Id == command.AlgoTaskId);

        if (algoTask is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.NotFound.ToString(), "AlgoTask not found",
                "The algo task with such id does not exist. Therefore addition cannot be made.");
        }

        var supportsSuchLanguageAlready =
            algoTask.CodeSnippets.Any(c => c.LanguageId == command.InitialCodeSnippet.LanguageId);

        if (supportsSuchLanguageAlready)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.AlreadyExists.ToString(), "Already exists",
                "The algo task already has such language support. Therefore addition cannot be made.");
        }
        
        var language = await _db.CodeLanguages.FindAsync(command.InitialCodeSnippet.LanguageId);
        
        if (language is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}.{EntityName.CodeLanguage}", Errors.NotFound.ToString(), "CodeLanguage not found",
                "The code language with such id does not exist or is not supported. Therefore addition cannot be made.");
        }

        var isAlgoTaskPassable = await ValidateAlgoTaskIsPassable(
            command.InitialCodeSnippet.InitialSolutionCode,
            command.InitialCodeSnippet.TestsCode,
            language);

        if (!isAlgoTaskPassable)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.WrongFlow.ToString(), "Not passable",
                "The initial solution does not pass the tests. Therefore language support cannot be added.");
        }
        
        var codeSnippet = new AlgoTaskCodeSnippet
        {
            Language = language,
            SampleCode = command.InitialCodeSnippet.SampleCode,
            TestsCode = command.InitialCodeSnippet.TestsCode
        };
        
        algoTask.CodeSnippets.Add(codeSnippet); 
        await _db.SaveChangesAsync();

        return algoTask.ToResponse(GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, false); // TODO: This is usual behaviour across the app, but maybe I should fail the entire transaction also in case of view mapping problem?
    }
    
    public async Task<IEnumerable<AlgoTaskResponse>> GetAlgoTasks()
    {
        var query = _db.AlgoTasks
            .Include(t=>t.AlgoCategory)
            .Include(t=>t.CodeSnippets)
                .ThenInclude(c=>c.Language)
            .Include(t => t.PassedBy);
        
        if (!_currentUser.IsAuthenticated)
        {
            return await query
                .Select(t => t.ToResponse(GetTaskSupportedLanguages(t), t.CodeSnippets, false))
                .AsSplitQuery()
                .ToListAsync();
        }
        
        return await query
            .Select(t => t.ToResponse(GetTaskSupportedLanguages(t), t.CodeSnippets, t.PassedBy.Any(u=>u.Id == _currentUser.UserId.Value)))
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<AlgoTaskResponse> GetAlgoTaskById(Guid id)
    {
        var algoTask = await _db.AlgoTasks
            .Include(t=>t.AlgoCategory)
            .Include(t=>t.CodeSnippets)
                .ThenInclude(c=>c.Language)
            .Include(t => t.PassedBy)
            .SingleOrDefaultAsync(t=>t.Id == id);
        
        if (algoTask is null)
        {
            throw new IqpException(
                EntityName.AlgoTask,Errors.NotFound.ToString(), "Not found", "The algorithm task with such id does not exist.");
        }

        if (!_currentUser.IsAuthenticated) return algoTask.ToResponse(GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, false);
        var isPassed = algoTask.PassedBy.Any(u => u.Id == _currentUser.UserId);
        return algoTask.ToResponse(GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, isPassed);
    }
    
    public async Task<AlgoTaskResponse> UpdateAlgoTask(UpdateAlgoTaskCommand command)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }
        
        var algoTask = await _db.AlgoTasks
            .Include(t=>t.AlgoCategory)
            .Include(t=>t.CodeSnippets)
                .ThenInclude(c=>c.Language)
            .Include(t=>t.PassedBy)
            .SingleOrDefaultAsync(t=>t.Id == command.Id);

        if (algoTask is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.NotFound.ToString(), "AlgoTask not found",
                "The algo task with such id does not exist. Therefore update cannot be made.");
        }
        
        var titleAlreadyExists = await _db.AlgoTasks.AnyAsync(c => c.Title == command.Title && c.Id != command.Id);
        if(titleAlreadyExists)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.AlreadyExists.ToString(), "Already exists",
                "The algo task with such title already exists. Therefore update cannot be made.");
        }
        
        var newAlgoCategory = await _db.AlgoTaskCategories.FindAsync(command.AlgoCategoryId);
        if(newAlgoCategory is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}.{EntityName.AlgoCategory}", Errors.NotFound.ToString(), "AlgoCategory not found",
                "The algo category with such id does not exist. Therefore update cannot be made.");
        }

        algoTask.Title = command.Title;
        algoTask.Description = command.Description;
        algoTask.AlgoCategoryId = command.AlgoCategoryId;
        
        await _db.SaveChangesAsync();
        
        var isPassed = algoTask.PassedBy.Any(u => u.Id == _currentUser.UserId);
        return algoTask.ToResponse(GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, isPassed);
    }

    public async Task<TestRun> RunTestsOnCode(RunTestsOnCodeCommand command)
    {
        var commandValidationResult = _runTestsOnCodeCommandValidator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask, commandValidationResult.ToDictionary()); // Maybe make subentity?
        }
        
        var language = await _db.CodeLanguages.FindAsync(command.LanguageId);
        
        if (language is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}.{EntityName.CodeLanguage}", Errors.NotFound.ToString(), "CodeLanguage not found",
                "The code language with such id does not exist or is not supported. Therefore submission cannot be tested.");
        }
        
        var result = await _codeFileTestRunner
            .RunTestsOnCode(
                command.Code,
                command.Tests,
                language.Slug,
                Guid.NewGuid().ToString());
        
        return result;
    }

    public async Task<TestRun> TestAlgoTaskSolution(SubmitAlgoTaskSolutionCommand submissionCommand)
    { 
        var commandValidationResult = _submitCodeCommandValidator.Validate(submissionCommand);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask, commandValidationResult.ToDictionary()); // Maybe make subentity?
        }
        
        var algoTask = await _db.AlgoTasks
            .Include(t=>t.CodeSnippets)
                .ThenInclude(s=>s.Language)
            .SingleOrDefaultAsync(t=>t.Id == submissionCommand.AlgoTaskId);

        if (algoTask is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.NotFound.ToString(), "AlgoTask not found",
                "The algo task with such id does not exist. Therefore submission cannot be tested.");
        }

        var specifiedLanguageSnippet = algoTask.CodeSnippets.SingleOrDefault(s => s.LanguageId == submissionCommand.LanguageId);

        if (specifiedLanguageSnippet is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.WrongFlow.ToString(), "Language not supported", // TODO: Look for better error code
                "Specified language is not supported for this task.");
        }
        
        _logger.LogInformation("Running tests on code. Task: {task}, Language: {language}", algoTask.Id, specifiedLanguageSnippet.Language.Name);
        
        var result = await _codeFileTestRunner
            .RunTestsOnCode(
                submissionCommand.Code,
                specifiedLanguageSnippet.TestsCode,
                specifiedLanguageSnippet.Language.Slug,
                Guid.NewGuid().ToString());

        return result;
    }

    public async Task<TestRun> SubmitAlgoTaskSolution(SubmitAlgoTaskSolutionCommand submissionCommand)
    {
        var commandValidationResult = _submitCodeCommandValidator.Validate(submissionCommand);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask, commandValidationResult.ToDictionary()); // Maybe make subentity?
        }
        
        var algoTask = await _db.AlgoTasks
            .Include(t=>t.CodeSnippets)
                .ThenInclude(s=>s.Language)
            .Include(t=>t.PassedBy)
            .SingleOrDefaultAsync(t=>t.Id == submissionCommand.AlgoTaskId);

        if (algoTask is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.NotFound.ToString(), "AlgoTask not found",
                "The algo task with such id does not exist. Therefore submission cannot be tested.");
        }

        var specifiedLanguageSnippet = algoTask.CodeSnippets.SingleOrDefault(s => s.LanguageId == submissionCommand.LanguageId);

        if (specifiedLanguageSnippet is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.WrongFlow.ToString(), "Language not supported", // TODO: Look for better error code
                "Specified language is not supported for this task.");
        }
        
        var user = await _db.Users.FindAsync(_currentUser.UserId.Value);
       
        _logger.LogInformation("Running tests on code. Task: {task}, Language: {language}, User: {username}", algoTask.Id, specifiedLanguageSnippet.Language.Name, user.UserName);
        
        var result = await _codeFileTestRunner
            .RunTestsOnCode(
                submissionCommand.Code,
                specifiedLanguageSnippet.TestsCode,
                specifiedLanguageSnippet.Language.Slug,
                user.UserName);
        
        if (result.Status is TestStatus.Pass && !algoTask.PassedBy.Any(u=>u.Id == user.Id))
        {
            algoTask.PassedBy.Add(user);
            await _db.SaveChangesAsync();
        }
        
        return result;
    }
}