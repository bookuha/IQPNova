namespace IQP.Web.ViewModels.Questions;

public class CreateCommentaryRequest
{
    public required string Content { get; set; }
    public Guid? ReplyToId { get; set; }
}