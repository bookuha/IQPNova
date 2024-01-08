namespace IQP.Web.ViewModels.CodeLanguages;

public class CreateCodeLanguageRequest
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Extension { get; set; }
}