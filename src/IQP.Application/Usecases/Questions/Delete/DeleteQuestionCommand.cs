using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IQP.Application.Usecases.Questions.Delete;

public record DeleteQuestionCommand : IRequest<QuestionResponse>
{
    public required Guid Id { get; set; }
}

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, QuestionResponse>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteQuestionCommandHandler> _logger;

    public DeleteQuestionCommandHandler(IqpDbContext db, ICurrentUserService currentUser, ILogger<DeleteQuestionCommandHandler> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<QuestionResponse> Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
    {
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

        if (question.Commentaries.Any())
        {
            throw new IqpException(
                EntityName.Question, Errors.Restricted.ToString(), "Restricted", "You are not allowed to delete this question because it already has commentaries.");
        }

        _db.Remove(question);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId} has been deleted", question.Id);
        
        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
    }
}