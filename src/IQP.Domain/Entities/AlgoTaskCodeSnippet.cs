namespace IQP.Domain.Entities;

public class AlgoTaskCodeSnippet
{
    public Guid Id { get; set; }
    
    public Guid AlgoTaskId { get; set; }
    public AlgoTask AlgoTask { get; set; }
    
    public Guid LanguageId { get; set; }
    public CodeLanguage Language { get; set; }
    
    public required string SampleCode { get; set; }
    public required string TestsCode { get; set; }
}