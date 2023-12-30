using IQP.Application.Contracts.AlgoTasks.Utility;

namespace IQP.Application.Contracts.AlgoTasks.Commands;

public class CreateAlgoTaskCommand
{
    public CreateAlgoTaskCommand()
    {
    }
    
    public CreateAlgoTaskCommand(string title, string description, Guid algoCategoryId, CodeSnippet initialCodeSnippet)
    {
        Title = title;
        Description = description;
        AlgoCategoryId = algoCategoryId;
        InitialCodeSnippet = initialCodeSnippet;
    }
 
    
   public required string Title { get; set; }
   public required string Description { get; set; }
   public required Guid AlgoCategoryId { get; set; }
   // BL: We need at least 1 code snippet to define an AlgoTask
   public required CodeSnippet InitialCodeSnippet { get; set; }
   
}