using FluentValidation;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.Questions.Update;

public record UpdateQuestionCommand : IRequest<QuestionResponse>
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
}

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, QuestionResponse>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateQuestionCommandHandler> _logger;
    private readonly IValidator<UpdateQuestionCommand> _validator;


    public UpdateQuestionCommandHandler(IqpDbContext db, ICurrentUserService currentUser, ILogger<UpdateQuestionCommandHandler> logger, IValidator<UpdateQuestionCommand> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
        _validator = validator;
    }

    public async Task<QuestionResponse> Handle(UpdateQuestionCommand command, CancellationToken cancellationToken)
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
                "The category with such id does not exist. Therefore question cannot be updated.");
        }

        var question = await 
            _db.Questions
                .Include(q=>q.Category)
                .Include(q=>q.LikedBy)
                .Include(q=>q.Commentaries)
                .Include(q=>q.Creator)
                .SingleOrDefaultAsync(q => q.Id == command.Id);
        
        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }
        
        if (_currentUser.UserId != question.CreatorId)
        {
            throw new IqpException(
                EntityName.Question, Errors.Restricted.ToString(), "Restricted",
                "You are not allowed to delete this question.");
        }

        question.Title = command.Title;
        question.Description = command.Description;
        question.CategoryId = command.CategoryId;
        
        _db.Update(question);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId} has been updated", question.Id);

        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
    }
}