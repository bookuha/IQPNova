namespace IQP.Infrastructure.CodeRunner;

public interface IFileSolutionTestRunner
{
    Task<TestRun> RunTestsAsync(string solutionPath, ExecutorCodeLanguage language);
}