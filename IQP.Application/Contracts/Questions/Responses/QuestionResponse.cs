using IQP.Domain.Entities;

namespace IQP.Application.Contracts.Questions.Responses;

public class QuestionResponse
{
    public QuestionResponse()
    {
    }
    
    public QuestionResponse(Guid id, DateTime created, string title, string description, Guid categoryId, Guid creatorId)
    {
        Id = id;
        Created = created;
        Title = title;
        Description = description;
        CategoryId = categoryId;
        CreatorId = creatorId;
    }

    public required Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; } // Slug?
    public required Guid CreatorId { get; set; }
}

public static partial class QuestionMappingExtensions
{
    public static QuestionResponse ToResponse(this Question question)
    {
        return new QuestionResponse
        {
            Id = question.Id,
            Created = question.Created,
            Title = question.Title,
            Description = question.Description,
            CategoryId = question.CategoryId,
            CreatorId = question.CreatorId
        };
    }
}