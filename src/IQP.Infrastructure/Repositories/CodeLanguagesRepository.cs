using IQP.Domain.Entities;
using IQP.Domain.Entities.AlgoTasks;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IQP.Infrastructure.Repositories;

public class CodeLanguagesRepository : ICodeLanguagesRepository
{
    private readonly IqpDbContext _dbContext;

    public CodeLanguagesRepository(IqpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CodeLanguage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.CodeLanguages.SingleOrDefaultAsync(l => l.Id == id, cancellationToken: cancellationToken);
    }

    public Task<List<CodeLanguage>> GetAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.CodeLanguages.ToListAsync(cancellationToken: cancellationToken);
    }

    public void Add(CodeLanguage codeLanguage)
    {
        _dbContext.CodeLanguages.Add(codeLanguage);
    }

    public void Update(CodeLanguage codeLanguage)
    {
        _dbContext.CodeLanguages.Update(codeLanguage);
    }

    public void Delete(CodeLanguage codeLanguage)
    {
           _dbContext.CodeLanguages.Remove(codeLanguage);
    }

    public Task<bool> ExistsAsync(string name, string slug, string extension, CancellationToken cancellationToken = default)
    {
        return _dbContext.CodeLanguages
            .AnyAsync(l => 
                l.Name == name || 
                l.Slug == slug || 
                l.Extension == extension, cancellationToken: cancellationToken);
    }

    public Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Database
            .SqlQueryRaw<Guid>("""SELECT "LanguageId" AS "Value" FROM "AlgoTaskCodeSnippet" """)
            .AnyAsync(lId => lId == id, cancellationToken: cancellationToken);
    }
}