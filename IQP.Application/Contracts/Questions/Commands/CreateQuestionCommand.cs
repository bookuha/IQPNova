using IQP.Domain.Entities;

namespace IQP.Application.Contracts.Questions.Commands;

public class CreateQuestionCommand
{
    public CreateQuestionCommand()
    {
    }

    public CreateQuestionCommand(string title, string description, Guid categoryId, Guid creatorId)
    {
        Title = title;
        Description = description;
        CategoryId = categoryId;
        CreatorId = creatorId;
    }

    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
    public required Guid CreatorId { get; set; }
}