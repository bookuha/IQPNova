using IQP.Application.Contracts.CodeLanguages;
using IQP.Application.Contracts.CodeLanguages.Commands;
using IQP.Application.Contracts.CodeLanguages.Responses;

namespace IQP.Application.Services;

public interface ICodeLanguagesService
{
    public Task<CodeLanguageResponse> CreateLanguage(CreateCodeLanguageCommand command);
    public Task<IEnumerable<CodeLanguageResponse>> GetLanguages();
    public Task<CodeLanguageResponse> GetLanguage(Guid id);
    public Task<CodeLanguageResponse> UpdateLanguage(UpdateCodeLanguageCommand command);
    public Task<CodeLanguageResponse> DeleteLanguage(Guid id);
}