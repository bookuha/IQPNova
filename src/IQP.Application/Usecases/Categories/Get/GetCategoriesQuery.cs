using IQP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.Categories.Get;

public record GetCategoriesQuery : IRequest<IEnumerable<CategoryResponse>>
{
    
}

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>>
{
    private readonly IqpDbContext _db;

    public GetCategoriesQueryHandler(IqpDbContext db)
    {
        _db = db;
    }
    
    public async Task<IEnumerable<CategoryResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _db.Categories
            .Select(c => c.ToResponse())
            .ToListAsync();

        return categories;
    }
}
