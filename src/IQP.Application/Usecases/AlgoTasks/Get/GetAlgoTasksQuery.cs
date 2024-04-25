using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.AlgoTasks.Get;

public record GetAlgoTasksQuery : IRequest<IEnumerable<AlgoTaskResponse>>
{
}

public class GetAlgoTasksQueryHandler : IRequestHandler<GetAlgoTasksQuery, IEnumerable<AlgoTaskResponse>>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;


    public async Task<IEnumerable<AlgoTaskResponse>> Handle(GetAlgoTasksQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.AlgoTasks
            .Include(t => t.AlgoCategory)
            .Include(t => t.CodeSnippets)
            .ThenInclude(c => c.Language)
            .Include(t => t.PassedBy);

        if (!_currentUser.IsAuthenticated)
        {
            return await query
                .Select(t => t.ToResponse(Functions.GetTaskSupportedLanguages(t), t.CodeSnippets, false))
                .AsSplitQuery()
                .ToListAsync();
        }

        return await query
            .Select(t => t.ToResponse(Functions.GetTaskSupportedLanguages(t), t.CodeSnippets,
                t.PassedBy.Any(u => u.Id == _currentUser.UserId.Value)))
            .AsSplitQuery()
            .ToListAsync();
    }
}