namespace IQP.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    
    public required string Title { get; set; }
    public required string Description { get; set; }

    public ICollection<Question> Questions { get; set; } = new HashSet<Question>();
}