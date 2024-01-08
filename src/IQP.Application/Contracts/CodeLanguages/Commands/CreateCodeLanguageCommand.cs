namespace IQP.Application.Contracts.CodeLanguages.Commands;

public class CreateCodeLanguageCommand
{
    public CreateCodeLanguageCommand()
    {
    }

    public CreateCodeLanguageCommand(string name, string slug, string extension)
    {
        Name = name;
        Slug = slug;
        Extension = extension;
    }

    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Extension { get; set; }
}