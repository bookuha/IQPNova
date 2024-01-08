using IQP.Domain.Entities;

namespace IQP.Web.ViewModels.AlgoTasks;

public class SubmitCodeRequest
{
    public Guid LanguageId { get; set; }
    public required string Code { get; set; }
}