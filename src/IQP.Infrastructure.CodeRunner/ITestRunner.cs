﻿namespace IQP.Infrastructure.CodeRunner;

public interface ITestRunner
{
    public Task<TestRun> RunTestsOnCode(string solutionCode, string testsCode, string languageSlug,
        string username);
    public Task<TestRun> RunTestsOnCode(string solutionCode, string testsCode, ExecutorCodeLanguage codeLanguage,
        string username);
}