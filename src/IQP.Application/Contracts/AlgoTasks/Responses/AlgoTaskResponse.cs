using IQP.Application.Contracts.AlgoTasks.Utility;
using IQP.Domain.Entities;

namespace IQP.Application.Contracts.AlgoTasks.Responses;

public class AlgoTaskResponse
{
    public required Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid AlgoCategoryId { get; set; }
    public required string AlgoCategoryTitle { get; set; }
    public IEnumerable<CodeLanguageResponse> SupportedLanguages { get; set; } = new HashSet<CodeLanguageResponse>();
    public IEnumerable<SampleCodeDto> CodeSamples { get; set; } = new HashSet<SampleCodeDto>();
    public bool IsPassed { get; set; }
}

public class SampleCodeDto
{
    public required CodeLanguage Language { get; set; }
    public required string SampleCode { get; set; }
}

public static partial class AlgoTaskMappingExtensions
{
    public static AlgoTaskResponse ToResponse(
        this AlgoTask algoTask,
        IEnumerable<CodeLanguage> supportedLanguages,
        IEnumerable<AlgoTaskCodeSnippet> codeSnippets,
        bool isPassed = false )
    {
        return new AlgoTaskResponse
        {
            Id = algoTask.Id,
            Created = algoTask.Created,
            Title = algoTask.Title,
            Description = algoTask.Description,
            AlgoCategoryId = algoTask.AlgoCategoryId,
            AlgoCategoryTitle = algoTask.AlgoCategory.Title,
            SupportedLanguages = supportedLanguages.Select(cl=> cl.ToResponse()),
            CodeSamples = codeSnippets.Select(cs=> new SampleCodeDto
            {
                Language = cs.Language,
                SampleCode = cs.SampleCode
            }),
            IsPassed = isPassed
        };
    }
    
    public static AlgoTaskResponse ToResponse(
        this AlgoTask algoTask,
        IEnumerable<CodeLanguage> supportedLanguages,
        IEnumerable<SampleCodeDto> codeSamples,
        bool isPassed = false )
    {
        return new AlgoTaskResponse
        {
            Id = algoTask.Id,
            Created = algoTask.Created,
            Title = algoTask.Title,
            Description = algoTask.Description,
            AlgoCategoryId = algoTask.AlgoCategoryId,
            AlgoCategoryTitle = algoTask.AlgoCategory.Title,
            SupportedLanguages = supportedLanguages.Select(cl=>cl.ToResponse()),
            CodeSamples = codeSamples,
            IsPassed = isPassed
        };
    }
    
}