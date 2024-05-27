using IQP.Domain.Entities;
using IQP.Domain.Entities.AlgoTasks;

namespace IQP.Domain.Repositories;

public interface IAlgoCategoriesRepository
{
    Task<AlgoTaskCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AlgoTaskCategory>> GetAsync(CancellationToken cancellationToken = default);
    void Add(AlgoTaskCategory algoCategory);
    void Update(AlgoTaskCategory algoCategory);
    void Delete(AlgoTaskCategory algoCategory);
    Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default);
    
}