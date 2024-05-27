using IQP.Domain.Exceptions;

namespace IQP.Domain.Entities.AlgoTasks;

public class AlgoTaskCodeSnippet
{
    public Guid Id { get; private  set; } = Guid.NewGuid();
    
    public Guid AlgoTaskId { get; private  set; }
    public AlgoTask AlgoTask { get; private  set; }
    
    public Guid LanguageId { get; private  set; }
    public CodeLanguage Language { get; private  set; }
    
    public string SampleCode { get; private  set; }  = null!;
    public string TestsCode { get; private  set; } = null!;
    
    public static AlgoTaskCodeSnippet Create(string sampleCode, string testsCode, CodeLanguage language)
    {
        Validate(sampleCode, testsCode, language);
        
        return new AlgoTaskCodeSnippet
        {
            SampleCode = sampleCode,
            TestsCode = testsCode,
            Language = language
        };
    }
    
    public void Update(string sampleCode, string testsCode, CodeLanguage language)
    {
        Validate(sampleCode, testsCode, language);
        
        SampleCode = sampleCode;
        TestsCode = testsCode;
        Language = language;
    }
    
    private static void Validate(string sampleCode, string testsCode, CodeLanguage language)
    {
        var validationProblems = new Dictionary<string, string[]>();

        if (string.IsNullOrEmpty(sampleCode) || sampleCode.Length > 1000)
        {
            validationProblems.Add("sampleCode", new[] {"Sample code must be at most 2000 characters long and not empty."});
        }

        if (string.IsNullOrEmpty(testsCode) ||  testsCode.Length > 3000)
        {
            validationProblems.Add("testsCode", new[] {"Tests code must be at most 3000 characters long and not empty."});
        }

        if (language is null)
        {
            validationProblems.Add("language", new[] {"Language must be provided."});
        }

        if (validationProblems.Any())
        {
            throw new ValidationException(EntityName.AlgoTask, validationProblems);
        }
    }
}