namespace IQP.Infrastructure.CodeRunner;

// This should be in Application layer, but it's here for simplicity
public class SlugToExecutorCodeLanguageConverter : ISlugToExecutorCodeLanguageConverter
{
    public ExecutorCodeLanguage Convert(string slug)
    {
        return slug switch
        {
            "csharp" => ExecutorCodeLanguage.Csharp,
            "fsharp" => ExecutorCodeLanguage.Fsharp,
            "java" => ExecutorCodeLanguage.Java,
            _ => throw new ArgumentException("Invalid slug", nameof(slug))
        };
    }
    
}