using IQP.Domain.Entities;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IQP.Infrastructure.Repositories;

public class AlgoTaskRepository : IAlgoTasksRepository
{
    private readonly IqpDbContext _dbContext;

    public AlgoTaskRepository(IqpDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<AlgoTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.AlgoTasks
            .Include(t=> t.AlgoCategory)
            .Include(t=> t.CodeSnippets)
            .ThenInclude(c=>c.Language)
            .Include(t => t.PassedBy)
            .SingleOrDefaultAsync(t=> t.Id == id, cancellationToken: cancellationToken);
    }

    public Task<List<AlgoTask>> GetAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.AlgoTasks
            .Include(t => t.AlgoCategory)
            .Include(t => t.CodeSnippets)
            .ThenInclude(c => c.Language)
            .Include(t => t.PassedBy)
            .AsSplitQuery()
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public void Add(AlgoTask algoTask)
    {
        _dbContext.AlgoTasks.Add(algoTask);
    }

    public void Update(AlgoTask algoTask)
    {
        _dbContext.AlgoTasks.Update(algoTask);
    }

    public void Delete(AlgoTask algoTask)
    {
        _dbContext.AlgoTasks.Remove(algoTask);
    }

    public Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default)
    {
        return _dbContext.AlgoTasks.AnyAsync(c => c.Title == title, cancellationToken);
    }
    
    public Task<bool> TitleExistsExceptInAsync(string title, Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.AlgoTasks.AnyAsync(c => c.Title == title && c.Id != id, cancellationToken);
    }
}