namespace IQP.Application.Contracts.Questions.Commands;

public class UpdateQuestionCommand
{
    public UpdateQuestionCommand()
    {
    }

    public UpdateQuestionCommand(Guid id, string title, string description, Guid categoryId)
    {
        Id = id;
        Title = title;
        Description = description;
        CategoryId = categoryId;
    }

    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
}