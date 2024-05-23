using System.Linq.Expressions;
using IQP.Domain.Entities;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Shared;
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

    public Task<PagedList<AlgoTask>> GetAsync(string? searchTerm, Guid? algoCategoryId, string? sortColumn, string? sortOrder, int? page, int? pageSize, CancellationToken cancellationToken = default)
    {
        IQueryable<AlgoTask> algoTasksQuery = _dbContext.AlgoTasks;

        if (algoCategoryId is not null)
        {
            algoTasksQuery = algoTasksQuery.Where(a => a.AlgoCategoryId == algoCategoryId);
        }
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            algoTasksQuery = algoTasksQuery.Where(a =>
                a.Title.Contains(searchTerm));
        }
        
        if (sortOrder?.ToLower() == "asc")
        {
            algoTasksQuery = algoTasksQuery.OrderBy(GetSortProperty(sortColumn));
        }
        else
        {
            algoTasksQuery = algoTasksQuery.OrderByDescending(GetSortProperty(sortColumn));
        }

        algoTasksQuery = algoTasksQuery
            .Include(t => t.AlgoCategory)
            .Include(t => t.CodeSnippets)
            .ThenInclude(c => c.Language)
            .Include(t => t.PassedBy)
            .AsSplitQuery();
        
        return PagedList<AlgoTask>.CreateFromQueryAsync(algoTasksQuery, page, pageSize);
    }
    
    private static Expression<Func<AlgoTask, object>> GetSortProperty(string? sortColumn) =>
        sortColumn?.ToLower() switch
        {
            "title" => algoTask => algoTask.Title,
            "passed" => algoTask => algoTask.PassedBy.Count,
            "date" => question => question.Created,
            _ => question => question.Created
        };


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