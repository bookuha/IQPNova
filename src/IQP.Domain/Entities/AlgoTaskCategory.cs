namespace IQP.Domain.Entities;

public class AlgoTaskCategory
{
    public Guid Id { get; set; }
    
    public required string Title { get; set; }
    public required string Description { get; set; }

    public ICollection<AlgoTask> AlgoTasks { get; set; } = new HashSet<AlgoTask>();
}