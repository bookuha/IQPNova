using IQP.Domain.Exceptions;

namespace IQP.Domain.Entities.AlgoTasks;

public class AlgoTaskCategory
{
    public Guid Id { get; private set; }  = Guid.NewGuid();
    
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public ICollection<AlgoTask> AlgoTasks { get; private set; } = new HashSet<AlgoTask>();

    public static AlgoTaskCategory Create(string title, string description)
    {
        Validate(title, description);

        return new AlgoTaskCategory
        {
            Title = title,
            Description = description
        };
    }

    public void Update(string title, string description)
    {
        Validate(title, description);

        Title = title;
        Description = description;
    }

    private static void Validate(string title, string description)
    {
        var validationProblems = new Dictionary<string, string[]>();

        if (string.IsNullOrEmpty(title) || title.Length < 4 || title.Length > 30)
        {
            validationProblems.Add("title", new[] {"Title must be between 4 and 30 characters long and not empty."});
        }

        if (string.IsNullOrEmpty(description) || description.Length < 4 || description.Length > 120)
        {
            validationProblems.Add("description",
                new[] {"Description must be between 4 and 120 characters long and not empty."});
        }

        if (validationProblems.Any())
        {
            throw new ValidationException(EntityName.AlgoCategory, validationProblems);
        }
    }
}