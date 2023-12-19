namespace IQP.Web.ViewModels;

public class CreateCommentaryRequest
{
    public required string Content { get; set; }
    public Guid? ReplyToId { get; set; }
}