using IQP.Domain.Entities;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IQP.Infrastructure.Repositories;

public class AlgoCategoriesRepository : IAlgoCategoriesRepository
{
    private readonly IqpDbContext _dbContext;

    public AlgoCategoriesRepository(IqpDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<AlgoTaskCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.AlgoTaskCategories.SingleOrDefaultAsync(c => c.Id == id,
            cancellationToken: cancellationToken);
    }

    public Task<List<AlgoTaskCategory>> GetAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.AlgoTaskCategories.ToListAsync(cancellationToken: cancellationToken);
    }

    public void Add(AlgoTaskCategory algoCategory)
    {
        _dbContext.AlgoTaskCategories.Add(algoCategory);
    }

    public void Update(AlgoTaskCategory algoCategory)
    {
        _dbContext.AlgoTaskCategories.Update(algoCategory);
    }

    public void Delete(AlgoTaskCategory algoCategory)
    {
        _dbContext.AlgoTaskCategories.Remove(algoCategory);
    }

    public Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default)
    {
        return _dbContext.AlgoTaskCategories.AnyAsync(c => c.Title == title, cancellationToken: cancellationToken);
    }
}