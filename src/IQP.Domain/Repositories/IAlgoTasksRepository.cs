using IQP.Domain.Entities;

namespace IQP.Domain.Repositories;

public interface IAlgoTasksRepository
{
    Task<AlgoTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AlgoTask>> GetAsync(CancellationToken cancellationToken = default);
    void Add(AlgoTask algoTask);
    void Update(AlgoTask algoTask);
    void Delete(AlgoTask algoTask);
    Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default);
    public Task<bool> TitleExistsExceptInAsync(string title, Guid id, CancellationToken cancellationToken = default);
}