using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using IQP.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.AlgoTasks.Get;

public record GetAlgoTasksQuery(
    string? SearchTerm,
    Guid? AlgoCategoryId,
    string? SortColumn,
    string? SortOrder,
    int Page,
    int PageSize) : IRequest<PagedList<AlgoTaskResponse>>;

public class GetAlgoTasksQueryHandler : IRequestHandler<GetAlgoTasksQuery, PagedList<AlgoTaskResponse>>
{
    private readonly IAlgoTasksRepository _algoTasksRepository;
    private readonly ICurrentUserService _currentUser;
    
    public GetAlgoTasksQueryHandler(IAlgoTasksRepository algoTasksRepository, ICurrentUserService currentUser)
    {
        _algoTasksRepository = algoTasksRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedList<AlgoTaskResponse>> Handle(GetAlgoTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _algoTasksRepository.GetAsync(request.SearchTerm, request.AlgoCategoryId, request.SortColumn,
            request.SortOrder, request.Page, request.PageSize);
        if (!_currentUser.IsAuthenticated)
        {
            return tasks.Map(t => t.ToResponse(Functions.GetTaskSupportedLanguages(t), t.CodeSnippets, false));
        }

        return tasks.Map(t => t.ToResponse(Functions.GetTaskSupportedLanguages(t), t.CodeSnippets,
            t.PassedBy.Any(u => u.Id == _currentUser.UserId.Value)));
    }
}