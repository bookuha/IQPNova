namespace IQP.Application.Contracts.AlgoTasks.Commands;

public class UpdateAlgoTaskCommand
{
    public UpdateAlgoTaskCommand()
    {
    }
    
    public UpdateAlgoTaskCommand(Guid id, string title, string description, Guid algoCategoryId)
    {
        Id = id;
        Title = title;
        Description = description;
        AlgoCategoryId = algoCategoryId;
    }
 
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid AlgoCategoryId { get; set; }
 
}