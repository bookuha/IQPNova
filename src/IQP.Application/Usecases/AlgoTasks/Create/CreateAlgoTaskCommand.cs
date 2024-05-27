using FluentValidation;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Entities.AlgoTasks;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.CodeRunner;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.AlgoTasks.Create;

public record CreateAlgoTaskCommand : IRequest<AlgoTaskResponse>
{
   public required string Title { get; set; }
   public required string Description { get; set; }
   public required Guid AlgoCategoryId { get; set; }
   public required CodeSnippet InitialCodeSnippet { get; set; }
}

public class CreateAlgoTaskCommandHandler : IRequestHandler<CreateAlgoTaskCommand, AlgoTaskResponse>
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IAlgoTasksRepository _algoTasksRepository;
   private readonly IAlgoCategoriesRepository _algoCategoriesRepository;
   private readonly ICodeLanguagesRepository _codeLanguagesRepository;
   private readonly IUserService _userService;
   private readonly ICurrentUserService _currentUser;
   private readonly ITestRunnerService _testRunner;
   private readonly IValidator<CreateAlgoTaskCommand> _validator;
   
   public CreateAlgoTaskCommandHandler(IUnitOfWork unitOfWork, IAlgoTasksRepository algoTasksRepository, IAlgoCategoriesRepository algoCategoriesRepository, ICodeLanguagesRepository codeLanguagesRepository, IUserService userService, ICurrentUserService currentUser, ITestRunnerService testRunner, IValidator<CreateAlgoTaskCommand> validator)
   {
       _unitOfWork = unitOfWork;
       _algoTasksRepository = algoTasksRepository;
       _algoCategoriesRepository = algoCategoriesRepository;
       _codeLanguagesRepository = codeLanguagesRepository;
       _userService = userService;
       _currentUser = currentUser;
       _testRunner = testRunner;
       _validator = validator;
   }

   public async Task<AlgoTaskResponse> Handle(CreateAlgoTaskCommand command, CancellationToken cancellationToken)
   {
       if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
       {
           throw IqpException.NotAdmin();
       }
       
       var commandValidationResult = _validator.Validate(command);

       if (!commandValidationResult.IsValid)
       {
           throw new ValidationException(EntityName.AlgoTask,
               commandValidationResult.ToDictionary());
       }

       var titleAlreadyExists = await _algoTasksRepository.TitleExistsAsync(command.Title);

       if (titleAlreadyExists)
       {
           throw new IqpException(
               EntityName.AlgoTask, Errors.AlreadyExists.ToString(), "Already exists",
               "The algo task with such title already exists.");
       }

       var algoCategory = await _algoCategoriesRepository.GetByIdAsync(command.AlgoCategoryId, cancellationToken);

       if (algoCategory is null)
       {
           throw new IqpException(
               $"{EntityName.AlgoCategory}", Errors.NotFound.ToString(), "AlgoCategory not found",
               "The algo category with such id does not exist. Therefore algo task cannot be created.");
       }

       var language = await _codeLanguagesRepository.GetByIdAsync(command.InitialCodeSnippet.LanguageId, cancellationToken);

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

       var initialTestSuite = new TestSuite(command.InitialCodeSnippet.SampleCode, command.InitialCodeSnippet.TestsCode,
           language);
   

       var algoTask = AlgoTask.Create(command.Title, command.Description, algoCategory, initialTestSuite);
       
       
       _algoTasksRepository.Add(algoTask);
       await _unitOfWork.SaveChangesAsync(cancellationToken);

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