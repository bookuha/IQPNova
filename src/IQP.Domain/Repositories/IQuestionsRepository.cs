using IQP.Domain.Entities;

namespace IQP.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Question>> GetAsync(CancellationToken cancellationToken = default);
    void Add(Question question);
    void Update(Question question);
    void Delete(Question question);
    public Task<List<Commentary>> GetCommentariesByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
}