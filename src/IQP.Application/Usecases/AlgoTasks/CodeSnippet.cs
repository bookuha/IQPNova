namespace IQP.Application.Usecases.AlgoTasks;

public class CodeSnippet
{
    public required Guid LanguageId { get; set; }
    public required string InitialSolutionCode { get; set; }
    public required string TestsCode { get; set; }
    public required string SampleCode { get; set; }
}