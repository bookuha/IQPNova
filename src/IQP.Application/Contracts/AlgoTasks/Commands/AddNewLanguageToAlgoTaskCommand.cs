using IQP.Application.Contracts.AlgoTasks.Utility;

namespace IQP.Application.Contracts.AlgoTasks.Commands;

public class AddNewLanguageToAlgoTaskCommand
{
    public AddNewLanguageToAlgoTaskCommand()
    {
        
    }
    
    public AddNewLanguageToAlgoTaskCommand(Guid algoTaskId, CodeSnippet initialCodeSnippet)
    {
        AlgoTaskId = algoTaskId;
        InitialCodeSnippet = initialCodeSnippet;
    }
    public Guid AlgoTaskId { get; set; }
    public required CodeSnippet InitialCodeSnippet { get; set; }
}