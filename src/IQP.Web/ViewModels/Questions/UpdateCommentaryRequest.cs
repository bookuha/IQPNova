namespace IQP.Web.ViewModels.Questions;

public class UpdateCommentaryRequest
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
}