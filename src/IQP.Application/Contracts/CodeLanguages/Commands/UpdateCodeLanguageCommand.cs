namespace IQP.Application.Contracts.CodeLanguages.Commands;

public class UpdateCodeLanguageCommand
{
    public UpdateCodeLanguageCommand()
    {
    }

    public UpdateCodeLanguageCommand(Guid id, string name, string slug, string extension)
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