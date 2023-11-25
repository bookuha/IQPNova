using IQP.Application.Contracts.Categories.Commands;
using IQP.Application.Contracts.Categories.Responses;

namespace IQP.Application.Services;

public interface ICategoriesService
{
    public Task<CategoryResponse> CreateCategory(CreateCategoryCommand command);
    public Task<IEnumerable<CategoryResponse>> GetCategories();
    public Task<CategoryResponse> GetCategoryById(Guid id);
    public Task<CategoryResponse> UpdateCategory(UpdateCategoryCommand command);
    public Task<CategoryResponse> DeleteCategory(Guid id);
}