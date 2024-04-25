using IQP.Domain.Entities;

namespace IQP.Application.Usecases.Commentaries;

public class CommentaryResponse
{
    public CommentaryResponse()
    {
    }
    
    public CommentaryResponse(Guid id, DateTime created, string content, Guid? parentId, Guid questionId, Guid creatorId, string creatorName, int likesCount, bool isLiked, IEnumerable<CommentaryResponse> replies)
    {
        Id = id;
        Created = created;
        Content = content;
        ParentId = parentId;
        QuestionId = questionId;
        CreatorId = creatorId;
        CreatorName = creatorName;
        LikesCount = likesCount;
        CommentariesCount = replies.Count();
        IsLiked = isLiked;
        Replies = replies;
    }

    public required Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public required string Content { get; set; }
    public Guid? ParentId { get; set; }
    public required Guid QuestionId { get; set; }
    public required Guid CreatorId { get; set; }
    public required string CreatorName { get; set; }
    public required int LikesCount { get; set; }
    public required int CommentariesCount {get; set; }
    public bool IsLiked { get; set; }
    public required IEnumerable<CommentaryResponse> Replies { get; set; }
}

public static partial class CommentaryMappingExtensions
{
    public static CommentaryResponse ToResponse(this Commentary commentary, bool isLiked = false)
    {
        return new CommentaryResponse
        {
            Id = commentary.Id,
            Created = commentary.Created,
            Content = commentary.Content,
            ParentId = commentary.ReplyTo?.Id,
            QuestionId = commentary.QuestionId,
            CreatorId = commentary.CreatedById,
            CreatorName = commentary.CreatedBy.UserName,
            LikesCount = commentary.LikedBy.Count,
            CommentariesCount = commentary.Replies.Count,
            Replies = commentary.Replies.Select(c => c.ToResponse()),
            IsLiked = isLiked
        };
    }
}