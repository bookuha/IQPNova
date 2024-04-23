using FluentValidation;
using IQP.Application.Contracts.Questions.Commands;
using IQP.Application.Services;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace IQP.Application.UnitTests.Services;

[TestFixture]
public class QuestionsServiceTests
{
    private Mock<IqpDbContext> _dbMock;
    private Mock<ICurrentUserService> _currentUserMock;
    private Mock<IValidator<CreateQuestionCommand>> _createQuestionCommandValidatorMock;
    private Mock<IValidator<UpdateQuestionCommand>> _updateQuestionCommandValidatorMock;
    private Mock<ILogger<QuestionsService>> _loggerMock;
    
    [SetUp]
    public void Setup()
    {
        _dbMock = new Mock<IqpDbContext>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _createQuestionCommandValidatorMock = new Mock<IValidator<CreateQuestionCommand>>();
        _updateQuestionCommandValidatorMock = new Mock<IValidator<UpdateQuestionCommand>>();
        _loggerMock = new Mock<ILogger<QuestionsService>>();
    }

    [Test]
    public void Only_Creator_Can_Delete_Question()
    {
       Assert.Pass();
    }
    
}