using IQP.Domain.Entities;
using IQP.Domain.Entities.AlgoTasks;
using IQP.Shared;

namespace IQP.Domain.Repositories;

public interface IAlgoTasksRepository
{
    Task<AlgoTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedList<AlgoTask>> GetAsync(string? searchTerm, Guid? algoCategoryId, string? sortColumn, string? sortOrder,
        int? page, int? pageSize, CancellationToken cancellationToken = default);
    void Add(AlgoTask algoTask);
    void Update(AlgoTask algoTask);
    void Delete(AlgoTask algoTask);
    Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default);
    public Task<bool> TitleExistsExceptInAsync(string title, Guid id, CancellationToken cancellationToken = default);
}