using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using MediatR;

namespace IQP.Application.Usecases.Categories.GetById;

public record GetCategoryByIdQuery : IRequest<CategoryResponse>
{
    public required Guid Id { get; init; }
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly ICategoriesRepository _categoriesRepository;

    public GetCategoryByIdQueryHandler(ICategoriesRepository categoriesRepository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoriesRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (category is null)
        {
            throw new IqpException(
                EntityName.Category,Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }
        
        return category.ToResponse();
    }
}