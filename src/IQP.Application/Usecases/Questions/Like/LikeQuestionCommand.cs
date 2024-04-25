using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.Questions.Like;

public record LikeQuestionCommand : IRequest<QuestionResponse>
{
    public required Guid Id { get; init; }
}

public class LikeQuestionCommandHandler : IRequestHandler<LikeQuestionCommand, QuestionResponse>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;


    public LikeQuestionCommandHandler(IqpDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<QuestionResponse> Handle(LikeQuestionCommand command, CancellationToken cancellationToken)
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
        
        var currentUser = await _db.Users.FindAsync(_currentUser.UserId);

        var isLikedAlready = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        if (isLikedAlready)
        {
            question.LikedBy.Remove(currentUser);
        }
        else
        {
            question.LikedBy.Add(currentUser);
        }
        await _db.SaveChangesAsync();
        
        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
    }
}