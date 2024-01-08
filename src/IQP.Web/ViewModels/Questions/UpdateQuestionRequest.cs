namespace IQP.Web.ViewModels.Questions;

public class UpdateQuestionRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
}