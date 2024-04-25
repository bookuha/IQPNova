using FluentValidation;
using FluentValidation.Results;
using IQP.Application.Contracts.AlgoTasks.Commands;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Application.Usecases.AlgoTasks;
using IQP.Application.Usecases.AlgoTasks.Create;
using IQP.Application.Usecases.AlgoTasks.SubmitSolution;
using IQP.Application.Usecases.AlgoTasks.Translate;
using IQP.Application.Usecases.AlgoTasks.Update;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.CodeRunner;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace IQP.Application.UnitTests.Services;

[TestFixture]
public class AlgoTaskServiceTests
{
    private Mock<IqpDbContext> _dbContextMock;
    private Mock<ITestRunnerService> _testRunnerMock;
    private Mock<ICurrentUserService> _currentUserMock;
    private Mock<IValidator<RunTestsOnCodeCommand>> _runTestsOnCodeCommandValidatorMock;
    private Mock<IValidator<SubmitAlgoTaskSolutionCommand>> _submitCodeCommandValidatorMock;
    private Mock<IValidator<CreateAlgoTaskCommand>> _createAlgoTaskCommandValidatorMock;
    private Mock<IValidator<UpdateAlgoTaskCommand>> _updateAlgoTaskCommandValidatorMock;
    private Mock<IValidator<TranslateAlgoTaskCommand>> _addNewLanguageToAlgoTaskCommandValidatorMock;
    private Mock<ILogger<AlgoTasksService>> _loggerMock;
    private Mock<IUserService> _userServiceMock;
    private AlgoTasksService _algoTaskService;

    [SetUp]
    public void Setup()
    {
        _dbContextMock = new Mock<IqpDbContext>();
        _testRunnerMock = new Mock<ITestRunnerService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _runTestsOnCodeCommandValidatorMock = new Mock<IValidator<RunTestsOnCodeCommand>>();
        _submitCodeCommandValidatorMock = new Mock<IValidator<SubmitAlgoTaskSolutionCommand>>();
        _createAlgoTaskCommandValidatorMock = new Mock<IValidator<CreateAlgoTaskCommand>>();
        _updateAlgoTaskCommandValidatorMock = new Mock<IValidator<UpdateAlgoTaskCommand>>();
        _addNewLanguageToAlgoTaskCommandValidatorMock = new Mock<IValidator<TranslateAlgoTaskCommand>>();
        _loggerMock = new Mock<ILogger<AlgoTasksService>>();
        _userServiceMock = new Mock<IUserService>();

        _algoTaskService = new AlgoTasksService(_dbContextMock.Object, _testRunnerMock.Object, _currentUserMock.Object,
            _runTestsOnCodeCommandValidatorMock.Object, _submitCodeCommandValidatorMock.Object,
            _createAlgoTaskCommandValidatorMock.Object, _updateAlgoTaskCommandValidatorMock.Object,
            _addNewLanguageToAlgoTaskCommandValidatorMock.Object, _loggerMock.Object, _userServiceMock.Object);
    }

    [Test]
    public void Only_Admin_Can_Create_Algo_Task()
    {
        // Arrange
        var command = new CreateAlgoTaskCommand
        {
            Title = "Test",
            Description = "Test",
            AlgoCategoryId = Guid.NewGuid(),
            InitialCodeSnippet = new CodeSnippet
            {
                LanguageId = Guid.NewGuid(),
                InitialSolutionCode = "Test",
                TestsCode = "Test",
                SampleCode = "Test"
            }
        };

        _currentUserMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _userServiceMock.Setup(x => x.IsUserAdmin(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync(Is.TypeOf<IqpException>()
                .And
                .Property("Error").EqualTo(Errors.Restricted.ToString()),
            () => _algoTaskService.CreateAlgoTask(command));
    }

    [Test]
    public void Cannot_Create_Algo_Task_With_Existing_Title()
    {
        const string existingTitle = "Existing Title";

        // Arrange
        var command = new CreateAlgoTaskCommand
        {
            Title = existingTitle,
            Description = "Test",
            AlgoCategoryId = Guid.NewGuid(),
            InitialCodeSnippet = new CodeSnippet
            {
                LanguageId = Guid.NewGuid(),
                InitialSolutionCode = "Test",
                TestsCode = "Test",
                SampleCode = "Test"
            }
        };

        _currentUserMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _userServiceMock.Setup(x => x.IsUserAdmin(It.IsAny<Guid>())).ReturnsAsync(true);
        _createAlgoTaskCommandValidatorMock.Setup(x => x.Validate(It.IsAny<CreateAlgoTaskCommand>()))
            .Returns(new ValidationResult());
        _dbContextMock.Setup(x => x.AlgoTasks).ReturnsDbSet(new List<AlgoTask>
        {
            new()
            {
                Title = existingTitle,
                Description = "Test"
            }
        });

        // Act & Assert
        Assert.ThrowsAsync(Is.TypeOf<IqpException>()
                .And
                .Property("Error").EqualTo(Errors.AlreadyExists.ToString()),
            () => _algoTaskService.CreateAlgoTask(command));
    }
}