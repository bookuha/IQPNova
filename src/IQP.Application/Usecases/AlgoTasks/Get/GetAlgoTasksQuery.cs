using IQP.Domain.Repositories;
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
    private readonly IAlgoTasksRepository _algoTasksRepository;
    private readonly ICurrentUserService _currentUser;
    
    public GetAlgoTasksQueryHandler(IAlgoTasksRepository algoTasksRepository, ICurrentUserService currentUser)
    {
        _algoTasksRepository = algoTasksRepository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<AlgoTaskResponse>> Handle(GetAlgoTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _algoTasksRepository.GetAsync();
        if (!_currentUser.IsAuthenticated)
        {
            return tasks.Select(t => t.ToResponse(Functions.GetTaskSupportedLanguages(t), t.CodeSnippets, false));
        }

        return tasks.Select(t => t.ToResponse(Functions.GetTaskSupportedLanguages(t), t.CodeSnippets,
            t.PassedBy.Any(u => u.Id == _currentUser.UserId.Value)));
    }
}