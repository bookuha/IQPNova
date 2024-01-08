namespace IQP.Application.Contracts.AlgoTaskCategories.Commands;

public class CreateAlgoTaskCategoryCommand
{
    public CreateAlgoTaskCategoryCommand()
    {
    }

    public CreateAlgoTaskCategoryCommand(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public required string Title { get; set; }
    public required string Description { get; set; }
}