namespace IQP.Web.ViewModels.AlgoTasks;

public class UpdateAlgoTaskRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid AlgoCategoryId { get; set; }
}