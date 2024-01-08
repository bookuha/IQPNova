namespace IQP.Web.ViewModels.CodeLanguages;

public class UpdateCodeLanguageRequest
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Extension { get; set; }
}