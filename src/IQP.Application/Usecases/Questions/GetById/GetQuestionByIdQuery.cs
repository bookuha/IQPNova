using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.Questions.GetById;

public record GetQuestionByIdQuery : IRequest<QuestionResponse>
{
    public Guid Id { get; set; }
}

public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, QuestionResponse>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;


    public GetQuestionByIdQueryHandler(IqpDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<QuestionResponse> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await 
            _db.Questions
                .Include(q=>q.Category)        
                .Include(q=>q.LikedBy)
                .Include(q=>q.Commentaries)
                .Include(q=>q.Creator)
                .SingleOrDefaultAsync(q => q.Id == request.Id);

        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }

        if (!_currentUser.IsAuthenticated) return question.ToResponse();
        var isLiked = question.LikedBy.Any(u => u.Id == _currentUser.UserId);
        return question.ToResponse(isLiked);
    }
}
