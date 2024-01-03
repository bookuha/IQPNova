using IQP.Application.Contracts.AlgoTaskCategories.Commands;
using IQP.Application.Contracts.AlgoTaskCategories.Responses;

namespace IQP.Application.Services;

public interface IAlgoTaskCategoriesService
{
    public Task<AlgoTaskCategoryResponse> CreateCategory(CreateAlgoTaskCategoryCommand command);
    public Task<IEnumerable<AlgoTaskCategoryResponse>> GetCategories();
    public Task<AlgoTaskCategoryResponse> GetCategoryById(Guid id);
    public Task<AlgoTaskCategoryResponse> UpdateCategory(UpdateAlgoTaskCategoryCommand command);
    public Task<AlgoTaskCategoryResponse> DeleteCategory(Guid id);
}