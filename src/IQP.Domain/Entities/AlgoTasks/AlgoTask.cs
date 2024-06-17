using IQP.Application;
using IQP.Domain.Exceptions;

namespace IQP.Domain.Entities.AlgoTasks;

public class AlgoTask
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime Created { get; private set; }
    
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    
    public Guid AlgoCategoryId { get; private set; }
    public AlgoTaskCategory AlgoCategory { get; private set; }
    // Creator defined information for each possible language: sample code, tests code, etc.
    public ICollection<AlgoTaskCodeSnippet> CodeSnippets { get; private set; } = new HashSet<AlgoTaskCodeSnippet>();
    
    public ICollection<User> PassedBy { get; private set; } = new HashSet<User>();
    public HashSet<User> LikedBy { get; private set; } = new HashSet<User>();

    public static AlgoTask Create(string title, string description, AlgoTaskCategory category, TestSuite initialTestSuite)
    {
        Validate(title, description, category);
        
        var initialCodeSnippet = AlgoTaskCodeSnippet.Create(initialTestSuite.SampleCode, initialTestSuite.TestsCode, initialTestSuite.Language);
        
        return new AlgoTask
        {
            Title = title,
            Description = description,
            AlgoCategory = category,
            CodeSnippets = new List<AlgoTaskCodeSnippet> {initialCodeSnippet},
        };
    }

    public void Update(string title, string description, AlgoTaskCategory category)
    {
        Validate(title, description, category);

        Title = title;
        Description = description;
        AlgoCategory = category;
    }

    public void AddPassedBy(User user)
    {
        if (user is null)
        {
            // Is intended only for personal logging, not the end user.
            throw new ArgumentException("User cannot be null", nameof(user));
        }

        if (!PassedBy.Any(u => u.Id == user.Id))
        {
            PassedBy.Add(user);
        }
    }
    
    public void AddTranslation(TestSuite testSuite)
    {
        var supportsSuchLanguageAlready =
           CodeSnippets.Any(c => c.Language.Id == testSuite.Language.Id);

        if (supportsSuchLanguageAlready)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.AlreadyExists.ToString(), "Already exists",
                "The algo task already has such language support. Therefore addition cannot be made.");
        }
        
        var codeSnippet = AlgoTaskCodeSnippet.Create(testSuite.SampleCode, testSuite.TestsCode, testSuite.Language);
        
        CodeSnippets.Add(codeSnippet);
    }

    private static void Validate(string title, string description, AlgoTaskCategory category)
    {
        var validationProblems = new Dictionary<string, string[]>();

        if (string.IsNullOrEmpty(title) || title.Length < 4 || title.Length > 100)
        {
            validationProblems.Add("title", new[] { "Title must be between 4 and 100 characters long and not empty." });
        }

        if (string.IsNullOrEmpty(description) || description.Length < 4 || description.Length > 750)
        {
            validationProblems.Add("description", new[] { "Description must be between 4 and 750 characters long and not empty." });
        }

        if (category is null)
        {
            validationProblems.Add("algoCategory", new[] { "AlgoCategoryId must not be empty." });
        }

        if (validationProblems.Any())
        {
            throw new ValidationException(EntityName.AlgoTask, validationProblems);
        }
    }
}

public record TestSuite(string SampleCode, string TestsCode, CodeLanguage Language);