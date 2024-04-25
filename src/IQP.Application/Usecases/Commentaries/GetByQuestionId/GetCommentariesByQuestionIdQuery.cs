using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.Commentaries.GetByQuestionId;

public record GetCommentariesByQuestionIdQuery : IRequest<IEnumerable<CommentaryResponse>>
{
    public required Guid QuestionId { get; set; }
}

public class
    GetCommentariesByQuestionIdQueryHandler : IRequestHandler<GetCommentariesByQuestionIdQuery,
    IEnumerable<CommentaryResponse>>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetCommentariesByQuestionIdQueryHandler(IqpDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }


    public async Task<IEnumerable<CommentaryResponse>> Handle(GetCommentariesByQuestionIdQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.Commentaries
            .Where(c => c.QuestionId == request.QuestionId)
            .Include(c => c.Replies)
            .ThenInclude(r => r.CreatedBy)
            .Include(c => c.LikedBy)
            .Include(c => c.CreatedBy)
            .Where(c => c.ReplyTo == null);

        if (!_currentUser.IsAuthenticated)
        {
            return await query
                .Select(q => q.ToResponse(false))
                .AsSplitQuery()
                .ToListAsync();
        }

        return await query
            .Select(q => q.ToResponse(q.LikedBy.Any(u => u.Id == _currentUser.UserId)))
            .AsSplitQuery()
            .ToListAsync();
    }
}