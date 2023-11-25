namespace IQP.Application.Contracts.Categories.Commands;

public class CreateCategoryCommand
{
    public CreateCategoryCommand()
    {
    }

    public CreateCategoryCommand(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public required string Title { get; set; }
    public required string Description { get; set; }
}