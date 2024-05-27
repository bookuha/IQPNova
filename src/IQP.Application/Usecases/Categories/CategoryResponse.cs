using IQP.Domain.Entities;
using IQP.Domain.Entities.Questions;

namespace IQP.Application.Usecases.Categories;

public class CategoryResponse
{
    public CategoryResponse()
    {
    }

    public CategoryResponse(Guid id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
    }

    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    // TODO: Don't need to return questions right now.
}

public static partial class CategoryMappingExtensions
{
    public static CategoryResponse ToResponse(this Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Title = category.Title,
            Description = category.Description
        };
    }
}
