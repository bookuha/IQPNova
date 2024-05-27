using IQP.Domain.Entities;
using IQP.Domain.Entities.AlgoTasks;

namespace IQP.Domain.Repositories;

public interface ICodeLanguagesRepository
{
    public Task<CodeLanguage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<List<CodeLanguage>> GetAsync(CancellationToken cancellationToken = default);
    public void Add(CodeLanguage codeLanguage);
    public void Update(CodeLanguage codeLanguage);
    public void Delete(CodeLanguage codeLanguage);
    public Task<bool> ExistsAsync(string name, string slug, string extension, CancellationToken cancellationToken = default);
    public Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken = default);
}