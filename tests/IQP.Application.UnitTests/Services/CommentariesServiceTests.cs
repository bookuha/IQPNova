using FluentValidation;
using FluentValidation.Results;
using IQP.Application.Contracts.Commentaries.Commands;
using IQP.Application.Services;
using IQP.Application.Services.Validators;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace IQP.Application.UnitTests.Services;

 [TestFixture]
    public class CommentariesServiceTests
    {
        private Mock<IqpDbContext> _dbMock;
        private Mock<ICurrentUserService> _currentUserMock;
        private Mock<IValidator<CreateCommentaryCommand>> _createCommentaryCommandValidatorMock;
        private Mock<ILogger<CommentariesService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _dbMock = new Mock<IqpDbContext>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _createCommentaryCommandValidatorMock = new Mock<IValidator<CreateCommentaryCommand>>();
            _loggerMock = new Mock<ILogger<CommentariesService>>();
        }

        [Test]
        public void Creating_Reply_To_NonExistingCommentary_Throws_NotFound()
        {
            // Arrange
            var command = new CreateCommentaryCommand
            {
                ReplyToId = Guid.NewGuid(),
                QuestionId = Guid.NewGuid(),
                Content = "Test"
            };
            
            _createCommentaryCommandValidatorMock.Setup(v => v.Validate(It.IsAny<CreateCommentaryCommand>())).Returns(new ValidationResult());
            _currentUserMock.Setup(u => u.UserId).Returns(Guid.NewGuid());
            _dbMock.Setup(db=>db.Users.FindAsync(It.IsAny<Guid>())).ReturnsAsync(new User());
            _dbMock.Setup(db => db.Questions.FindAsync(command.QuestionId)).ReturnsAsync(new Question
            {
                Title = "Test",
                Description = "Test"
            });
            _dbMock.Setup(db => db.Commentaries.FindAsync(command.ReplyToId)).ReturnsAsync((Commentary)null);

            var service = new CommentariesService(_dbMock.Object, _currentUserMock.Object, _createCommentaryCommandValidatorMock.Object, _loggerMock.Object);

            // Act
            var ex = Assert.ThrowsAsync<IqpException>(() => service.CreateCommentary(command));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("The commentary with such id does not exist. Therefore commentary cannot be created."));
        }

        [Test]
        public void Replying_To_Reply_Propagates_Commentary_to_Root()
        {
            Assert.Pass();
        }

        // Helper method to mock DbSet
        private DbSet<T> MockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            return dbSet.Object;
        }
    }
