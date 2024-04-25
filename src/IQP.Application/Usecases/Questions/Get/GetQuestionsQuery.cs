using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.Questions.Get;

public record GetQuestionsQuery : IRequest<IEnumerable<QuestionResponse>>
{
    
}

public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, IEnumerable<QuestionResponse>>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetQuestionsQueryHandler(IqpDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<QuestionResponse>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Questions
            .Include(q => q.Category)
            .Include(q => q.LikedBy)
            .Include(q => q.Commentaries)
            .Include(q => q.Creator);
        
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