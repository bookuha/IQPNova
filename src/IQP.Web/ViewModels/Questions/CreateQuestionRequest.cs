namespace IQP.Web.ViewModels.Questions;

public class CreateQuestionRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
}