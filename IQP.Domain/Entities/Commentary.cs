namespace IQP.Domain.Entities;

public class Commentary
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    
    public required string Content { get; set; }
    
    public Guid? ReplyToId { get; set; }
    public Commentary? ReplyTo { get; set; }
    public HashSet<Commentary> Replies { get; set; } = new HashSet<Commentary>();

    public HashSet<User> LikedBy { get; set; } = new HashSet<User>();

    public Guid QuestionId { get; set; }
    public Question Question { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }
}