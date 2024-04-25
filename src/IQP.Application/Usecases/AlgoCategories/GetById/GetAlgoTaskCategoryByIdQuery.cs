using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using MediatR;

namespace IQP.Application.Usecases.AlgoCategories.GetById;

public record GetAlgoTaskCategoryByIdQuery : IRequest<AlgoTaskCategoryResponse>
{
    public required Guid Id { get; init; }
}

public class GetAlgoTaskCategoryByIdQueryHandler : IRequestHandler<GetAlgoTaskCategoryByIdQuery, AlgoTaskCategoryResponse>
{
    private readonly IAlgoCategoriesRepository _algoCategoriesRepository;

    public GetAlgoTaskCategoryByIdQueryHandler(IAlgoCategoriesRepository algoCategoriesRepository)
    {
        _algoCategoriesRepository = algoCategoriesRepository;
    }

    public async Task<AlgoTaskCategoryResponse> Handle(GetAlgoTaskCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _algoCategoriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            throw new IqpException(EntityName.AlgoCategory, Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }

        return category.ToResponse();
    }
}
