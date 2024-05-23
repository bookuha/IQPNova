using IQP.Domain.Entities;
using IQP.Shared;

namespace IQP.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedList<Question>> GetAsync(string? searchTerm, Guid? categoryId, string? sortColumn, string? sortOrder, int? page, int? pageSize, CancellationToken cancellationToken = default);
    void Add(Question question);
    void Update(Question question);
    void Delete(Question question);
    public Task<List<Commentary>> GetCommentariesByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
}