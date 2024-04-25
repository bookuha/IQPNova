using IQP.Domain.Entities;

namespace IQP.Domain.Repositories;

public interface ICategoriesRepository
{
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<List<Category>> GetAsync(CancellationToken cancellationToken = default);
    public void Add(Category category);
    public void Update(Category category);
    public void Delete(Category category);
    public Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default);
}