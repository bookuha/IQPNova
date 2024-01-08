namespace IQP.Domain.Entities;

public class AlgoTask
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    
    public required string Title { get; set; }
    public required string Description { get; set; }
    
    public Guid AlgoCategoryId { get; set; }
    public AlgoTaskCategory AlgoCategory { get; set; }
    // Creator defined information for each possible language: sample code, tests code, etc.
    public ICollection<AlgoTaskCodeSnippet> CodeSnippets { get; set; } = new HashSet<AlgoTaskCodeSnippet>();
    
    public ICollection<User> PassedBy { get; set; } = new HashSet<User>();
}