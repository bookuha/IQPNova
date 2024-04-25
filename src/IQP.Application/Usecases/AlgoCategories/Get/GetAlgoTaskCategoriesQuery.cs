using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.AlgoCategories.Get;

public record GetAlgoTaskCategoriesQuery : IRequest<IEnumerable<AlgoTaskCategoryResponse>>
{
    
}

public class GetAlgoTaskCategoriesQueryHandler : IRequestHandler<GetAlgoTaskCategoriesQuery, IEnumerable<AlgoTaskCategoryResponse>>
{
    private readonly IAlgoCategoriesRepository _algoCategoriesRepository;

    public GetAlgoTaskCategoriesQueryHandler(IAlgoCategoriesRepository algoCategoriesRepository)
    {
        _algoCategoriesRepository = algoCategoriesRepository;
    }


    public async Task<IEnumerable<AlgoTaskCategoryResponse>> Handle(GetAlgoTaskCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _algoCategoriesRepository.GetAsync(cancellationToken);

        return categories.Select(c => c.ToResponse());
    }
}