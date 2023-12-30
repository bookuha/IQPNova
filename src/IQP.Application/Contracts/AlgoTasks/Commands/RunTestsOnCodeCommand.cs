using IQP.Domain.Entities;

namespace IQP.Application.Contracts.AlgoTasks.Commands;

public class RunTestsOnCodeCommand
{
    public RunTestsOnCodeCommand()
    {
    }
    
    public RunTestsOnCodeCommand(Guid languageId, string code, string tests)
    {
        LanguageId = languageId;
        Code = code;
        Tests = tests;
    }
    
    public Guid LanguageId { get; set; }
    public required string Code { get; set; }
    public required string Tests { get; set; }
}