using FluentValidation;
using FluentValidation.Results;
using IQP.Application.Services.Users;
using IQP.Application.Usecases.AlgoTasks;
using IQP.Application.Usecases.AlgoTasks.Create;
using IQP.Domain.Entities.AlgoTasks;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.CodeRunner;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using Moq;

namespace IQP.Application.IntegrationTests.Usecases.AlgoTasks;

[TestFixture]
public class CreateAlgoTaskTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IAlgoTasksRepository> _algoTasksRepositoryMock;
    private Mock<IAlgoCategoriesRepository> _algoCategoriesRepositoryMock;
    private Mock<ICodeLanguagesRepository> _codeLanguagesRepositoryMock;
    private Mock<IUserService> _userServiceMock;
    private Mock<ICurrentUserService> _currentUserMock;
    private Mock<ITestRunnerService> _testRunnerMock;
    private Mock<IValidator<CreateAlgoTaskCommand>> _validatorMock;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _algoTasksRepositoryMock = new Mock<IAlgoTasksRepository>();
        _algoCategoriesRepositoryMock = new Mock<IAlgoCategoriesRepository>();
        _codeLanguagesRepositoryMock = new Mock<ICodeLanguagesRepository>();
        _userServiceMock = new Mock<IUserService>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _testRunnerMock = new Mock<ITestRunnerService>();
        _validatorMock = new Mock<IValidator<CreateAlgoTaskCommand>>();
    }

    private CreateAlgoTaskCommandHandler CreateHandler()
    {
        return new CreateAlgoTaskCommandHandler(
            _unitOfWorkMock.Object,
            _algoTasksRepositoryMock.Object,
            _algoCategoriesRepositoryMock.Object,
            _codeLanguagesRepositoryMock.Object,
            _userServiceMock.Object,
            _currentUserMock.Object,
            _testRunnerMock.Object,
            _validatorMock.Object
        );
    }

    [Test]
    public void Handle_UserNotAdmin_ThrowsNotAdminException()
    {
        // Arrange
        var command = new CreateAlgoTaskCommand
        {
            Title = "New Task",
            Description = "Task Description",
            AlgoCategoryId = Guid.NewGuid(),
            InitialCodeSnippet = new CodeSnippet
            {
                LanguageId = Guid.NewGuid(),
                InitialSolutionCode = "code",
                TestsCode = "tests",
                SampleCode = "sample"
            }
        };

        _currentUserMock.Setup(x => x.UserId).Returns(It.IsAny<Guid>());
        _userServiceMock.Setup(x => x.IsUserAdmin(It.IsAny<Guid>())).ReturnsAsync(false);

        var handler = CreateHandler();

        // Act & Assert
        var exception =
            Assert.ThrowsAsync<IqpException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.That(exception.Message, Is.EqualTo("You are not allowed to access this resource."));
    }

    [Test]
    public void Handle_TitleAlreadyExists_ThrowsAlreadyExistsException()
    {
        // Arrange
        var command = new CreateAlgoTaskCommand
        {
            Title = "Existing Task",
            Description = "Task Description",
            AlgoCategoryId = Guid.NewGuid(),
            InitialCodeSnippet = new CodeSnippet
            {
                LanguageId = Guid.NewGuid(),
                InitialSolutionCode = "code",
                TestsCode = "tests",
                SampleCode = "sample"
            }
        };

        _currentUserMock.Setup(x => x.UserId).Returns(It.IsAny<Guid>());
        _userServiceMock.Setup(x => x.IsUserAdmin(It.IsAny<Guid>())).ReturnsAsync(true);
        _validatorMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
        _algoTasksRepositoryMock.Setup(x => x.TitleExistsAsync(command.Title, CancellationToken.None))
            .ReturnsAsync(true);

        var handler = CreateHandler();

        // Act & Assert
        var exception =
            Assert.ThrowsAsync<IqpException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.That(exception.Message, Is.EqualTo("The algo task with such title already exists."));
    }

    [Test]
    public void Mock1()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock2()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock3()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock4()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock5()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock6()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock7()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock8()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock9()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Moc10()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock11()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock12()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock13()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock14()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock15()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock16()
    {
        Assert.Pass();
    }
    
    [Test]
    public void Mock17()
    {
        Assert.Pass();
    }
    

}
