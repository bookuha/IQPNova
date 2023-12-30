namespace IQP.Domain.Entities;

public class TechTask
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime ActiveUntil { get; set; } // TODO: Maybe specify number of days with int?
    
    public Guid CreatorId { get; set; }
    public User Creator { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } // We can declare TechTasks[] in Category and it will not affect the actual DB table structure, but I won't do this for now
    public HashSet<TechTaskSubmission> Submissions { get; set; }



}