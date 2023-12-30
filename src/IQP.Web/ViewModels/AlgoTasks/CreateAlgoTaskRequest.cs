using IQP.Domain.Entities;

namespace IQP.Web.ViewModels.AlgoTasks;

public class InitialCodeSnippet
{
    public required Guid LanguageId { get; set; }
    public required string InitialSolutionCode { get; set; }
    public required string TestsCode { get; set; }
    public required string SampleCode { get; set; }
}

public class CreateAlgoTaskRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid AlgoCategoryId { get; set; }
    public InitialCodeSnippet InitialCodeSnippet { get; set; }
    
}