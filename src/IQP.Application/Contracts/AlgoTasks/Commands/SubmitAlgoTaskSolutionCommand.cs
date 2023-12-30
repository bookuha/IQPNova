using IQP.Domain.Entities;

namespace IQP.Application.Contracts.AlgoTasks.Commands;

public class SubmitAlgoTaskSolutionCommand
{
    
    public SubmitAlgoTaskSolutionCommand()
    {
    }
    
    public SubmitAlgoTaskSolutionCommand(Guid algoTaskId, Guid languageId, string code)
    {
        AlgoTaskId = algoTaskId;
        LanguageId = languageId;
        Code = code;
    }
    
    public Guid AlgoTaskId { get; set; }
    public Guid LanguageId { get; set; }
    public required string Code { get; set; }
}