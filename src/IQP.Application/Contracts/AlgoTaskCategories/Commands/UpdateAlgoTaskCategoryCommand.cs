namespace IQP.Application.Contracts.AlgoTaskCategories.Commands;

public class UpdateAlgoTaskCategoryCommand
{
    public UpdateAlgoTaskCategoryCommand()
    {
    }

    public UpdateAlgoTaskCategoryCommand(Guid id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
    }

    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}