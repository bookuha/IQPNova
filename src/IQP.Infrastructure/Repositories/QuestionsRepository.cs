using System.Linq.Expressions;
using IQP.Domain.Entities;
using IQP.Domain.Entities.Questions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Shared;
using Microsoft.EntityFrameworkCore;

namespace IQP.Infrastructure.Repositories;

public class QuestionsRepository : IQuestionsRepository
{
    private readonly IqpDbContext _dbContext;

    public QuestionsRepository(IqpDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Questions
                .Include(q=>q.Category)
                .Include(q=>q.LikedBy)
                .Include(q=>q.Commentaries)
                .Include(q=>q.Creator)
                .SingleOrDefaultAsync(q => q.Id == id, cancellationToken: cancellationToken);
    }
    
    public Task<PagedList<Question>> GetAsync(string? searchTerm, Guid? categoryId, string? sortColumn, string? sortOrder, int? page, int? pageSize, CancellationToken cancellationToken = default)
    {
        IQueryable<Question> questionsQuery = _dbContext.Questions;

        if (categoryId is not null)
        {
            questionsQuery = questionsQuery.Where(q => q.CategoryId == categoryId);
        }
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            questionsQuery = questionsQuery.Where(q =>
                q.Title.Contains(searchTerm));
        }
        
        if (sortOrder?.ToLower() == "asc")
        {
            questionsQuery = questionsQuery.OrderBy(GetSortProperty(sortColumn));
        }
        else
        {
            questionsQuery = questionsQuery.OrderByDescending(GetSortProperty(sortColumn));
        }

        questionsQuery = questionsQuery
            .Include(q => q.Category)
            .Include(q => q.LikedBy)
            .Include(q => q.Commentaries)
            .Include(q => q.Creator)
            .AsSplitQuery();


        return PagedList<Question>.CreateFromQueryAsync(questionsQuery, page, pageSize);
    }
    
    private static Expression<Func<Question, object>> GetSortProperty(string? sortColumn) =>
        sortColumn?.ToLower() switch
        {
            "title" => question => question.Title,
            "likes" => question => question.LikedBy.Count,
            "commentaries" => question => question.Commentaries.Count,
            "date" => question => question.Created,
            _ => question => question.Id
        };

    public void Add(Question question)
    {
        _dbContext.Questions.Add(question);
    }

    public void Update(Question question)
    {
        _dbContext.Questions.Update(question);
    }

    public void Delete(Question question)
    {
        _dbContext.Questions.Remove(question);
    }
    
    public void AddCommentary(Question question, Commentary commentary)
    {
        // Once commentary has a GUID set before upon the creation, EF, when updating the Question, will treat it as an already existing entity, therefore we have to either:
        // 1. Preemptively add the commentary to the context and update the question.
        // 2. Delegate the ID creation to the DB, so unitialized GUIDs are treated as new entities.
        
        _dbContext.Commentaries.Add(commentary);
        question.Commentaries.Add(commentary);
        _dbContext.Questions.Update(question);
    }
    
    public Task<List<Commentary>> GetCommentariesByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Commentaries
            .Where(c => c.QuestionId == questionId)
            .Include(c => c.Replies)
            .ThenInclude(r => r.CreatedBy)
            .Include(c => c.LikedBy)
            .Include(c => c.CreatedBy)
            .Where(c => c.ReplyTo == null)
            .AsSplitQuery()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}