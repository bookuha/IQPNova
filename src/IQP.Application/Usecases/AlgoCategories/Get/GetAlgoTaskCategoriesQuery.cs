using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.AlgoCategories.Get;

public record GetAlgoTaskCategoriesQuery : IRequest<IEnumerable<AlgoTaskCategoryResponse>>
{
    
}

public class GetAlgoTaskCategoriesQueryHandler : IRequestHandler<GetAlgoTaskCategoriesQuery, IEnumerable<AlgoTaskCategoryResponse>>
{
    private readonly IqpDbContext _db;

    public GetAlgoTaskCategoriesQueryHandler(IqpDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AlgoTaskCategoryResponse>> Handle(GetAlgoTaskCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _db.AlgoTaskCategories
            .Select(c => c.ToResponse())
            .ToListAsync();
    }
}