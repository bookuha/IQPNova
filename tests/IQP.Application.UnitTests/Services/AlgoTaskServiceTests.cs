using FluentValidation;
using FluentValidation.Results;
using IQP.Application.Contracts.AlgoTasks.Commands;
using IQP.Application.Contracts.AlgoTasks.Utility;
using IQP.Application.Services;
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
    private Mock<IqpDbContext> _dbContext;
    private Mock<ITestRunnerService> _testRunner;
    private Mock<ICurrentUserService> _currentUser;
    private Mock<IValidator<RunTestsOnCodeCommand>> _runTestsOnCodeCommandValidator;
    private Mock<IValidator<SubmitAlgoTaskSolutionCommand>> _submitCodeCommandValidator;
    private Mock<IValidator<CreateAlgoTaskCommand>> _createAlgoTaskCommandValidator;
    private Mock<IValidator<UpdateAlgoTaskCommand>> _updateAlgoTaskCommandValidator;
    private Mock<IValidator<AddNewLanguageToAlgoTaskCommand>> _addNewLanguageToAlgoTaskCommandValidator;
    private Mock<ILogger<AlgoTasksService>> _logger;
    private Mock<IUserService> _userService;
    private AlgoTasksService _algoTaskService;

    [SetUp]
    public void Setup()
    {
        _dbContext = new Mock<IqpDbContext>();
        _testRunner = new Mock<ITestRunnerService>();
        _currentUser = new Mock<ICurrentUserService>();
        _runTestsOnCodeCommandValidator = new Mock<IValidator<RunTestsOnCodeCommand>>();
        _submitCodeCommandValidator = new Mock<IValidator<SubmitAlgoTaskSolutionCommand>>();
        _createAlgoTaskCommandValidator = new Mock<IValidator<CreateAlgoTaskCommand>>();
        _updateAlgoTaskCommandValidator = new Mock<IValidator<UpdateAlgoTaskCommand>>();
        _addNewLanguageToAlgoTaskCommandValidator = new Mock<IValidator<AddNewLanguageToAlgoTaskCommand>>();
        _logger = new Mock<ILogger<AlgoTasksService>>();
        _userService = new Mock<IUserService>();

        _algoTaskService = new AlgoTasksService(_dbContext.Object, _testRunner.Object, _currentUser.Object,
            _runTestsOnCodeCommandValidator.Object, _submitCodeCommandValidator.Object,
            _createAlgoTaskCommandValidator.Object, _updateAlgoTaskCommandValidator.Object,
            _addNewLanguageToAlgoTaskCommandValidator.Object, _logger.Object, _userService.Object);
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

        _currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _userService.Setup(x => x.IsUserAdmin(It.IsAny<Guid>())).ReturnsAsync(false);

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

        _currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _userService.Setup(x => x.IsUserAdmin(It.IsAny<Guid>())).ReturnsAsync(true);
        _createAlgoTaskCommandValidator.Setup(x => x.Validate(It.IsAny<CreateAlgoTaskCommand>()))
            .Returns(new ValidationResult());
        _dbContext.Setup(x => x.AlgoTasks).ReturnsDbSet(new List<AlgoTask>
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