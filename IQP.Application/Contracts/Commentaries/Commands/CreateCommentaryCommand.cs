namespace IQP.Application.Contracts.Commentaries.Commands;

public class CreateCommentaryCommand
{
    public CreateCommentaryCommand()
    {
        
    }
    
    public CreateCommentaryCommand(Guid questionId, string content, Guid? replyToId)
    {
        QuestionId = questionId;
        Content = content;
        ReplyToId = replyToId;
    }

    public required Guid QuestionId { get; set; }
    public required string Content { get; set; }
    public Guid? ReplyToId { get; set; }
}