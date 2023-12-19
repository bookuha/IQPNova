namespace IQP.Web.ViewModels;

public class UpdateCommentaryRequest
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
}