using IQP.Domain.Entities;
using IQP.Domain.Entities.AlgoTasks;

namespace IQP.Application.Usecases.AlgoTasks;

public static class Functions
{
    public static IEnumerable<CodeLanguage> GetTaskSupportedLanguages(AlgoTask algoTask) =>
        algoTask.CodeSnippets.Select(t => t.Language);
}