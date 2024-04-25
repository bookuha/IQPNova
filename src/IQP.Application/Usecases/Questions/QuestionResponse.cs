using IQP.Domain.Entities;

namespace IQP.Application.Usecases.Questions;

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
    public required Guid CategoryId { get; set; }
    public required string CategoryTitle { get; set; }
    public required Guid CreatorId { get; set; }
    public required string CreatorName { get; set; }
    // TODO: public required bool IsAuthor { get; set; }
    public required int LikesCount { get; set; }
    public required int CommentariesCount { get; set; }
    public bool IsLiked { get; set; }
}

public static partial class QuestionMappingExtensions
{
    public static QuestionResponse ToResponse(this Question question, bool isLiked = false)
    {
        return new QuestionResponse
        {
            Id = question.Id,
            Created = question.Created,
            Title = question.Title,
            Description = question.Description,
            CategoryId = question.CategoryId,
            CategoryTitle = question.Category.Title,
            CreatorId = question.CreatorId,
            CreatorName = question.Creator?.UserName ?? "Undefined",
            LikesCount = question.LikedBy.Count,
            CommentariesCount = question.Commentaries.Count,
            IsLiked = isLiked
        };
    }
}