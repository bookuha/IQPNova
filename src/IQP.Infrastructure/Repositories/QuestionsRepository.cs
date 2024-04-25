using IQP.Domain.Entities;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
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

    public Task<List<Question>> GetAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Questions
            .Include(q => q.Category)
            .Include(q => q.LikedBy)
            .Include(q => q.Commentaries)
            .Include(q => q.Creator)
            .AsSplitQuery()
            .ToListAsync(cancellationToken: cancellationToken);
    }

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