namespace IQP.Application.Contracts.Categories.Commands;

public class UpdateCategoryCommand
{
    public UpdateCategoryCommand()
    {
    }

    public UpdateCategoryCommand(Guid id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
    }

    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}