using FluentValidation;
using IQP.Application.Services.Users;
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

namespace IQP.Application.Usecases.AlgoTasks.SubmitSolution;

public record SubmitAlgoTaskSolutionCommand : IRequest<TestRun>
{
    public Guid AlgoTaskId { get; set; }
    public Guid LanguageId { get; set; }
    public required string Code { get; set; }
}

public class SubmitAlgoTaskSolutionCommandHandler : IRequestHandler<SubmitAlgoTaskSolutionCommand, TestRun>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAlgoTasksRepository _algoTasksRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<SubmitAlgoTaskSolutionCommand> _validator;
    private readonly ITestRunnerService _testRunner;
    private readonly ILogger<SubmitAlgoTaskSolutionCommandHandler> _logger;

    public SubmitAlgoTaskSolutionCommandHandler(IUnitOfWork unitOfWork, IAlgoTasksRepository algoTasksRepository, IUserService userService, ICurrentUserService currentUser, IValidator<SubmitAlgoTaskSolutionCommand> validator, ITestRunnerService testRunner, ILogger<SubmitAlgoTaskSolutionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _algoTasksRepository = algoTasksRepository;
        _userService = userService;
        _currentUser = currentUser;
        _validator = validator;
        _testRunner = testRunner;
        _logger = logger;
    }

    public async Task<TestRun> Handle(SubmitAlgoTaskSolutionCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = _validator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask,
                commandValidationResult.ToDictionary());
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
                $"{EntityName.AlgoTask}", Errors.WrongFlow.ToString(),
                "Language not supported", // TODO: Look for better error code
                "Specified language is not supported for this task.");
        }

        var user = await _userService.GetUserByIdAsync(_currentUser.UserId.Value);

        _logger.LogInformation("Running tests on code. Task: {task}, Language: {language}, User: {username}",
            algoTask.Id, specifiedLanguageSnippet.Language.Name, user.UserName);

        var result = await _testRunner
            .RunTestsOnCode(
                command.Code,
                specifiedLanguageSnippet.TestsCode,
                specifiedLanguageSnippet.Language.Slug,
                user.UserName);

        if (result.Status is TestStatus.Pass && !algoTask.PassedBy.Any(u => u.Id == user.Id))
        {
            algoTask.PassedBy.Add(user);
            _algoTasksRepository.Update(algoTask);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}