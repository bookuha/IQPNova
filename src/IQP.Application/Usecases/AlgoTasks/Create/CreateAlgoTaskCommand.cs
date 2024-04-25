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
   private readonly IqpDbContext _db;
   private readonly IUserService _userService;
   private readonly ICurrentUserService _currentUser;
   private readonly ITestRunnerService _testRunner;
   private readonly IValidator<CreateAlgoTaskCommand> _validator;


   public CreateAlgoTaskCommandHandler(IqpDbContext db, IUserService userService, ICurrentUserService currentUser, ITestRunnerService testRunner, IValidator<CreateAlgoTaskCommand> validator)
   {
      _db = db;
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

       var titleAlreadyExists = await _db.AlgoTasks.AnyAsync(c => c.Title == command.Title);

       if (titleAlreadyExists)
       {
           throw new IqpException(
               EntityName.AlgoTask, Errors.AlreadyExists.ToString(), "Already exists",
               "The algo task with such title already exists.");
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