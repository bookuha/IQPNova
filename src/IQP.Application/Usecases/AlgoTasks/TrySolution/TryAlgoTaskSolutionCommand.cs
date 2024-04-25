using FluentValidation;
using IQP.Application.Usecases.AlgoTasks.SubmitSolution;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.CodeRunner;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.AlgoTasks.TrySolution;

public record TryAlgoTaskSolutionCommand : IRequest<TestRun>
{
    public Guid AlgoTaskId { get; set; }
    public Guid LanguageId { get; set; }
    public required string Code { get; set; }
}

public class TryAlgoTaskSolutionCommandHandler : IRequestHandler<TryAlgoTaskSolutionCommand, TestRun>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAlgoTasksRepository _algoTasksRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<TryAlgoTaskSolutionCommand> _validator;
    private readonly ITestRunnerService _testRunner;
    private readonly ILogger<SubmitAlgoTaskSolutionCommandHandler> _logger;

    public TryAlgoTaskSolutionCommandHandler(IUnitOfWork unitOfWork, IAlgoTasksRepository algoTasksRepository, ICurrentUserService currentUser, IValidator<TryAlgoTaskSolutionCommand> validator, ITestRunnerService testRunner, ILogger<SubmitAlgoTaskSolutionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _algoTasksRepository = algoTasksRepository;
        _currentUser = currentUser;
        _validator = validator;
        _testRunner = testRunner;
        _logger = logger;
    }

    public async Task<TestRun> Handle(TryAlgoTaskSolutionCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = _validator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask, commandValidationResult.ToDictionary()); // Maybe make subentity?
        }
        
        var algoTask = await _algoTasksRepository.GetByIdAsync(command.AlgoTaskId, cancellationToken);

        if (algoTask is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.NotFound.ToString(), "AlgoTask not found",
                "The algo task with such id does not exist. Therefore submission cannot be tested.");
        }

        var specifiedLanguageSnippet = algoTask.CodeSnippets.SingleOrDefault(s => s.LanguageId == command.LanguageId);

        if (specifiedLanguageSnippet is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.WrongFlow.ToString(), "Language not supported", // TODO: Look for better error code
                "Specified language is not supported for this task.");
        }
        
        _logger.LogInformation("Running tests on code. Task: {task}, Language: {language}", algoTask.Id, specifiedLanguageSnippet.Language.Name);
        
        var result = await _testRunner
            .RunTestsOnCode(
                command.Code,
                specifiedLanguageSnippet.TestsCode,
                specifiedLanguageSnippet.Language.Slug,
                Guid.NewGuid().ToString());

        return result;
    }
}