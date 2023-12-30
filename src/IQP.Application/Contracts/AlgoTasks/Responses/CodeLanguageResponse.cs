using IQP.Domain.Entities;

namespace IQP.Application.Contracts.AlgoTasks.Responses;

public class CodeLanguageResponse
{
    public CodeLanguageResponse()
    {
    }

    public CodeLanguageResponse(Guid id, string name, string slug, string extension)
    {
        Id = id;
        Name = name;
        Slug = slug;
        Extension = extension;
    }
    
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Extension { get; set; }
}

public static class CodeLanguageMappingExtensions
{
    public static CodeLanguageResponse ToResponse(this CodeLanguage codeLanguage) 
    {
        return new CodeLanguageResponse
        {
            Id = codeLanguage.Id,
            Name = codeLanguage.Name,
            Slug = codeLanguage.Slug,
            Extension = codeLanguage.Extension
        };
    }
}