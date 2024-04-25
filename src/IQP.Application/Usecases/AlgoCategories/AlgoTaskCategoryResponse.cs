using IQP.Domain.Entities;

namespace IQP.Application.Contracts.AlgoTaskCategories.Responses;

public class AlgoTaskCategoryResponse
{
    public AlgoTaskCategoryResponse()
    {
    }

    public AlgoTaskCategoryResponse(Guid id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
    }

    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}

public static partial class AlgoTaskCategoryMappingExtensions
{
    public static AlgoTaskCategoryResponse ToResponse(this AlgoTaskCategory category)
    {
        return new AlgoTaskCategoryResponse
        {
            Id = category.Id,
            Title = category.Title,
            Description = category.Description
        };
    }
}