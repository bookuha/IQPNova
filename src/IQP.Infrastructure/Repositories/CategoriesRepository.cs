using IQP.Domain.Entities;
using IQP.Domain.Entities.Questions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IQP.Infrastructure.Repositories;

public class CategoriesRepository : ICategoriesRepository
{
    private readonly IqpDbContext _dbContext;

    public CategoriesRepository(IqpDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .SingleOrDefaultAsync(c=>c.Id == id, cancellationToken: cancellationToken);
    }

    public Task<List<Category>> GetAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .AsSplitQuery()
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public void Add(Category category)
    {
        _dbContext.Categories.Add(category);
    }

    public void Update(Category category)
    {
        _dbContext.Categories.Update(category);
    }

    public void Delete(Category category)
    {
        _dbContext.Categories.Remove(category);
    }

    public Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.AnyAsync(c => c.Title == title, cancellationToken);
    }
}