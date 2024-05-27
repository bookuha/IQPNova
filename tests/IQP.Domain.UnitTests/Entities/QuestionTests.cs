using IQP.Domain.Entities;
using IQP.Domain.Entities.Questions;
using IQP.Domain.Exceptions;

namespace IQP.Domain.UnitTests.Entities;

[TestFixture]
public class QuestionTests
{
    private Category _validCategory;
    private User _validUser;
    private User _adminUser;

    [SetUp]
    public void SetUp()
    {
        _validCategory = Category.Create("Test Category", "The test category for a question.");
        _validUser = new User {Id = Guid.NewGuid(), Email = "user@test.com", UserName = "User", IsAdmin = false};
        _adminUser = new User {Id = Guid.NewGuid(), Email = "admin@test.com", UserName = "Admin", IsAdmin = true};
    }

    [Test]
    public void Can_Create_Valid_Question()
    {
        // Arrange
        const string title = "Valid Question Title";
        const string description = "This is a valid question description.";

        // Act
        var question = Question.Create(title, description, _validCategory, _validUser);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(question, Is.Not.Null);
            Assert.That(question.Title, Is.EqualTo(title));
            Assert.That(question.Description, Is.EqualTo(description));
            Assert.That(question.Category, Is.EqualTo(_validCategory));
            Assert.That(question.Creator, Is.EqualTo(_validUser));
        });
    }

    [Test]
    public void Cannot_Create_Question_With_Invalid_Title()
    {
        // Arrange
        const string invalidTitle = "Short";
        const string description = "This is a valid question description.";

        // Act
        var ex = Assert.Throws<ValidationException>(() =>
            Question.Create(invalidTitle, description, _validCategory, _validUser));

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Errors["title"][0],
            Is.EqualTo("Title must be between 10 and 100 characters long and not empty."));
    }

    [Test]
    public void Cannot_Create_Question_With_Invalid_Description()
    {
        // Arrange
        const string title = "Valid Question Title";
        const string invalidDescription = "Short description";

        // Act
        var ex = Assert.Throws<ValidationException>(() =>
            Question.Create(title, invalidDescription, _validCategory, _validUser));

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Errors["description"][0],
            Is.EqualTo("Description must be between 20 and 600 characters long and not empty."));
    }

    [Test]
    public void Can_Update_Question_With_Valid_Values()
    {
        // Arrange
        var question = Question.Create("Valid Question Title", "This is a valid question description.", _validCategory,
            _validUser);
        const string newTitle = "New Valid Title";
        const string newDescription = "New valid question description.";

        // Act
        question.Update(newTitle, newDescription, _validCategory, _validUser);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(question.Title, Is.EqualTo(newTitle));
            Assert.That(question.Description, Is.EqualTo(newDescription));
            Assert.That(question.Category, Is.EqualTo(_validCategory));
        });
    }

    [Test]
    public void Cannot_Update_Question_With_Invalid_Title()
    {
        // Arrange
        var question = Question.Create("Valid Question Title", "This is a valid question description.", _validCategory,
            _validUser);
        const string invalidTitle = "Short";

        // Act
        var ex = Assert.Throws<ValidationException>(() =>
            question.Update(invalidTitle, "New valid description.", _validCategory, _validUser));

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Errors["title"][0],
            Is.EqualTo("Title must be between 10 and 100 characters long and not empty."));
    }

    [Test]
    public void Cannot_Update_Question_By_Non_Creator_Or_Admin()
    {
        // Arrange
        var question = Question.Create("Valid Question Title", "This is a valid question description.", _validCategory,
            _validUser);
        var anotherUser = new User
            {Id = Guid.NewGuid(), Email = "another@test.com", UserName = "AnotherUser", IsAdmin = false};

        // Act
        var ex = Assert.Throws<IqpException>(() =>
            question.Update("New Title", "New Description", _validCategory, anotherUser));

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Is.EqualTo("You are not allowed to update this question."));
    }

    [Test]
    public void Can_Add_Comment_To_Question()
    {
        // Arrange
        var question = Question.Create("Valid Question Title", "This is a valid question description.", _validCategory,
            _validUser);
        const string commentContent = "This is a comment.";

        // Act
        var commentary = question.Comment(commentContent, _validUser);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(commentary, Is.Not.Null);
            Assert.That(commentary.Content, Is.EqualTo(commentContent));
            Assert.That(question.Commentaries, Contains.Item(commentary));
        });
    }

    /*[Test]
    public void Comment_On_NonRoot_Commentary_Is_Pushed_To_Root()
    {
        // Arrange
        var question = Question.Create("Valid Question Title", "This is a valid question description.", _validCategory,
            _validUser);
        const string rootCommentContent = "This is a root comment.";
        const string replyCommentContent = "This is a reply comment.";
        const string replyToReplyCommentContent = "This is a reply to reply comment.";

        // Act
        var rootCommentary = question.Comment(rootCommentContent, _validUser);
        var replyCommentary = question.Comment(replyCommentContent, _validUser, rootCommentary.Id);
        var replyToReplyCommentary = question.Comment(replyToReplyCommentContent, _validUser, replyCommentary.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rootCommentary, Is.Not.Null);
            Assert.That(replyCommentary, Is.Not.Null);
            Assert.That(rootCommentary.Content, Is.EqualTo(rootCommentContent));
            Assert.That(replyCommentary.Content, Is.EqualTo(replyCommentContent));
            Assert.That(question.Commentaries, Contains.Item(rootCommentary));
            Assert.That(question.Commentaries, Contains.Item(replyCommentary));
            Assert.That(question.Commentaries, Contains.Item(replyToReplyCommentary));
            
            Assert.That(replyCommentary.ReplyTo, Is.EqualTo(rootCommentary));
            
            Assert.That(replyToReplyCommentary.ReplyTo, Is.EqualTo(rootCommentary));
        });
        
    }*/

    [Test]
    public void Cannot_Comment_On_Nonexistent_Root_Commentary()
    {
        // Arrange
        var question = Question.Create("Valid Question Title", "This is a valid question description.", _validCategory,
            _validUser);
        const string commentContent = "This is a comment.";
        var nonexistentReplyToId = Guid.NewGuid();

        // Act
        var ex = Assert.Throws<IqpException>(() =>
            question.Comment(commentContent, _validUser, nonexistentReplyToId));

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message,
            Is.EqualTo("The root commentary with such id does not exist. Therefore commentary cannot be created."));
    }

    [Test]
    public void Can_Like_And_Unlike_Question()
    {
        // Arrange
        var question = Question.Create("Valid Question Title", "This is a valid question description.", _validCategory,
            _validUser);

        // Act
        question.Like(_validUser);

        // Assert
        Assert.That(question.LikedBy, Contains.Item(_validUser));

        // Act
        question.Like(_validUser);

        // Assert
        Assert.That(question.LikedBy, Does.Not.Contain(_validUser));
    }
}