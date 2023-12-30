namespace IQP.Application.Contracts.Commentaries.Commands;

public class UpdateCommentaryCommand
{
    public UpdateCommentaryCommand()
    {
    }
    
    public UpdateCommentaryCommand(Guid id, string content)
    {
        Id = id;
        Content = content;
    }
    
    public required Guid Id { get; set; }
    public required string Content { get; set; }
}