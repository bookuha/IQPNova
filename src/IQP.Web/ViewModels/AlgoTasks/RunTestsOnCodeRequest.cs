using IQP.Domain.Entities;

namespace IQP.Web.ViewModels.AlgoTasks;

public class RunTestsOnCodeRequest
{
    public Guid LanguageId { get; set; }
    public required string Code { get; set; }
    public required string Tests { get; set; }
}