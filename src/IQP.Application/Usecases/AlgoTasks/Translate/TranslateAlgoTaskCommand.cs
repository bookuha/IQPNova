using FluentValidation;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.CodeRunner;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.AlgoTasks.Translate;

public record TranslateAlgoTaskCommand : IRequest<AlgoTaskResponse>
{
    public Guid AlgoTaskId { get; set; }
    public required CodeSnippet InitialCodeSnippet { get; set; }
}

public class TranslateAlgoTaskCommandHandler : IRequestHandler<TranslateAlgoTaskCommand, AlgoTaskResponse>
{
    private readonly IqpDbContext _db;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly ITestRunnerService _testRunner;
    private readonly IValidator<TranslateAlgoTaskCommand> _validator;


    public TranslateAlgoTaskCommandHandler(IqpDbContext db, IUserService userService, ICurrentUserService currentUser, ITestRunnerService testRunner, IValidator<TranslateAlgoTaskCommand> validator)
    {
        _db = db;
        _userService = userService;
        _currentUser = currentUser;
        _testRunner = testRunner;
        _validator = validator;
    }

    public async Task<AlgoTaskResponse> Handle(TranslateAlgoTaskCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = _validator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask, commandValidationResult.ToDictionary());
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

        return algoTask.ToResponse(Functions.GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, false);
    }
    
    private async Task<bool> ValidateAlgoTaskIsPassable(string initialSolutionCode, string testsCode,
        CodeLanguage language)
    {
        var result =
            await _testRunner.RunTestsOnCode(initialSolutionCode, testsCode, language.Slug, Guid.NewGuid().ToString());

        return result.Status is TestStatus.Pass;
    }
}