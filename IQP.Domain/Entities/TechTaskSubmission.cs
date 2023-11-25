namespace IQP.Domain.Entities;

public class TechTaskSubmission
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    
    public required string Content { get; set; }
    public string? Review { get; set; } // Is filled once answer is given by tech task author
    
    public Guid TechTaskId { get; set; }
    public TechTask TechTask { get; set; }
    public Guid CreatorId { get; set; }
    public User Creator { get; set; }
}