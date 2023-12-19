namespace IQP.Domain.Entities;

public class Question
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    
    public required string Title { get; set; }
    public required string Description { get; set; }

    public HashSet<Commentary> Commentaries { get; set; } = new HashSet<Commentary>();
    public HashSet<User> LikedBy { get; set; } = new HashSet<User>();
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public Guid CreatorId { get; set; }
    public User Creator { get; set; }
}