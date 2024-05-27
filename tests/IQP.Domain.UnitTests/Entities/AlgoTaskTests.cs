using IQP.Domain.Entities;
using IQP.Domain.Entities.AlgoTasks;
using IQP.Domain.Exceptions;

namespace IQP.Domain.UnitTests.Entities;

[TestFixture]
public class AlgoTaskTests
{
    private AlgoTaskCategory _validCategory;
    private CodeLanguage _validCodeLanguage;
    private TestSuite _validInitialTestSuite;
    private User _validAdminUser;

    [SetUp]
    public void SetUp()
    {
        _validCategory = AlgoTaskCategory.Create("Test", "The test category for a task.");
        _validCodeLanguage = CodeLanguage.Create("Test", "test", ".test");
        _validInitialTestSuite = new TestSuite("Sample code", "Tests code", _validCodeLanguage);
        _validAdminUser = new User {Id = Guid.NewGuid(), Email = "test@test.com", IsAdmin = true, UserName = "Test"};
    }

    [Test]
    public void Can_Create_Valid_Algotask()
    {
        // Arrange
        const string title = "Valid Title";
        const string description = "Valid Description";

        // Act
        var algoTask = AlgoTask.Create("Valid Title", "Valid Description", _validCategory, _validInitialTestSuite);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(algoTask, Is.Not.Null);
            Assert.That(algoTask.Title, Is.EqualTo(title));
            Assert.That(algoTask.Description, Is.EqualTo(description));
            Assert.That(algoTask.AlgoCategory, Is.EqualTo(_validCategory));
            Assert.That(algoTask.CodeSnippets, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void Cannot_Create_AlgoTask_With_Invalid_Title()
    {
        // Arrange
        const string invalidTitle = "T";
        
        // Act
        var ex = Assert.Throws<ValidationException>(() =>
            AlgoTask.Create(invalidTitle, "Valid Description", _validCategory, _validInitialTestSuite));

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Errors["title"][0], Is.EqualTo("Title must be between 4 and 100 characters long and not empty."));
    }
    
    [Test]
    public void Can_Update_AlgoTask_With_Valid_Values()
    {
        // Arrange
        const string title = "Valid Title";
        const string description = "Valid Description";
        const string newTitle = "New Valid Title";
        const string newDescription = "New Valid Description";

        // Act
        var algoTask = AlgoTask.Create(title, description, _validCategory, _validInitialTestSuite); 
        // Category remains the same
        algoTask.Update(newTitle, newDescription, _validCategory);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(algoTask.Title, Is.EqualTo(newTitle));
            Assert.That(algoTask.Description, Is.EqualTo(newDescription));
            Assert.That(algoTask.AlgoCategory, Is.EqualTo(_validCategory));
        });
    }

    [Test]
    public void Cannot_Update_AlgoTask_With_Invalid_Title()
    {
        // Arrange
        const string invalidTitle = "T";
        var algoTask = AlgoTask.Create("Valid Title", "Valid Description", _validCategory, _validInitialTestSuite);

        // Act
        var ex = Assert.Throws<ValidationException>(() =>
            algoTask.Update(invalidTitle, "New Description", _validCategory));

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Errors["title"][0], Is.EqualTo("Title must be between 4 and 100 characters long and not empty."));
    }

    [Test]
    public void Can_Add_Passed_User()
    {
        // Arrange
        var algoTask = AlgoTask.Create("Valid Title", "Valid Description", _validCategory, _validInitialTestSuite);
        var user = new User {Id = Guid.NewGuid(), UserName = "test"};
        
        // Act
        algoTask.AddPassedBy(user);

        // Assert
        Assert.That(algoTask.PassedBy.Contains(user), Is.True);
    }
    
    [Test]
    public void Add_Passed_User_Wont_Duplicate_User()
    {
        // Arrange
        var algoTask = AlgoTask.Create("Valid Title", "Valid Description", _validCategory, _validInitialTestSuite);
        var user = new User {Id = Guid.NewGuid(), UserName = "test"};
        algoTask.AddPassedBy(user);
        
        // Act
        algoTask.AddPassedBy(user);

        Assert.That(algoTask.PassedBy, Has.Exactly(1).EqualTo(user));
    }
    
    [Test]
    public void Can_AddTranslation_With_New_Language()
    {
        // Arrange
        var algoTask = AlgoTask.Create("Valid Title", "Valid Description", _validCategory, _validInitialTestSuite);
        var newLanguage = CodeLanguage.Create("Qava", "qava", ".qava");
        var newTestSuite = new TestSuite("Sample code", "Tests code", newLanguage);
        
        // Act
        algoTask.AddTranslation(newTestSuite);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(algoTask.CodeSnippets, Has.One.Matches<AlgoTaskCodeSnippet>(c => c.Language == newLanguage));
            Assert.That(algoTask.CodeSnippets, Has.Exactly(2).Items);
        });
    }

    [Test]
    public void Cannot_AddTranslation_With_Existing_Language()
    {
        // Arrange
        var algoTask = AlgoTask.Create("Valid Title", "Valid Description", _validCategory, _validInitialTestSuite);
        var newTestSuiteWithSameLanguage = new TestSuite("Sample code", "Tests code", _validCodeLanguage);

        // Act
        var ex = Assert.Throws<IqpException>(() => algoTask.AddTranslation(newTestSuiteWithSameLanguage));
        
        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Is.EqualTo("The algo task already has such language support. Therefore addition cannot be made."));
    }
}