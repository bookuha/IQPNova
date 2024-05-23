using IQP.Infrastructure.Data;

namespace IQP.Infrastructure.Repositories;

// Just an abstraction over the DbContext SaveChangesAsync method. I don't want to have Save() methods in my repositories.
public class UnitOfWork : IUnitOfWork
{
    private readonly IqpDbContext _context;

    public UnitOfWork(IqpDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    
}