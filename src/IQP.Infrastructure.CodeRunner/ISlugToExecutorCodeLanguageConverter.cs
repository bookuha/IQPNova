namespace IQP.Infrastructure.CodeRunner;

public interface ISlugToExecutorCodeLanguageConverter
{
    public ExecutorCodeLanguage Convert(string slug);
}