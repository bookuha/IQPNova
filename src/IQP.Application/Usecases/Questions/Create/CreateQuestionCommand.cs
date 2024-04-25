using FluentValidation;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.Questions.Create;

public class CreateQuestionCommand : IRequest<QuestionResponse>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
}

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, QuestionResponse>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateQuestionCommandHandler> _logger;
    private readonly IValidator<CreateQuestionCommand> _validator;


    public CreateQuestionCommandHandler(IqpDbContext db, ICurrentUserService currentUser, ILogger<CreateQuestionCommandHandler> logger, IValidator<CreateQuestionCommand> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
        _validator = validator;
    }

    public async Task<QuestionResponse> Handle(CreateQuestionCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = _validator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Question, commandValidationResult.ToDictionary());
        }

        var category = await _db.Categories.FindAsync(command.CategoryId);
        
        if (category is null)
        {
            throw new IqpException(
                $"{EntityName.Question}.{EntityName.Category}", Errors.NotFound.ToString(), "Category not found",
                "The category with such id does not exist. Therefore question cannot be created.");
        }
        
        var user = await _db.Users.FindAsync(_currentUser.UserId.Value); // TODO: Add null check here / do something at all

        var question = new Question
        {
            Title = command.Title,
            Description = command.Description,
            Creator = user,
            CategoryId = command.CategoryId
        };

        _db.Add(question);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId}, {Title} has been created", question.Id, question.Title);

        return question.ToResponse();
    }
}