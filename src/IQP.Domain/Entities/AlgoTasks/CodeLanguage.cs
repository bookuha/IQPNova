using System.Text.RegularExpressions;
using IQP.Domain.Exceptions;

namespace IQP.Domain.Entities.AlgoTasks;

public class CodeLanguage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; }  = null!;
    public string Extension { get; private set; }
    
    public static CodeLanguage Create(string name, string slug, string extension)
    {
        Validate(name, slug, extension);

        return new CodeLanguage
        {
            Name = name,
            Slug = slug.ToLower(),
            Extension = extension.ToLower()
        };
    }
    
    public void Update(string name, string slug, string extension)
    {
        Validate(name, slug, extension);

        Name = name;
        Slug = slug.ToLower();
        Extension = extension.ToLower();
    }
    
    private static void Validate(string name, string slug, string extension)
    {
        var validationProblems = new Dictionary<string, string[]>();

        if (string.IsNullOrEmpty(name) || name.Length > 30)
        {
            validationProblems.Add("name", new[] {"Name must be at most 30 characters long and not empty."});
        }

        if (string.IsNullOrEmpty(slug) || slug.Length > 10)
        {
            validationProblems.Add("slug", new[] {"Slug must be at most 10 characters long and not empty."});
        }
        
        const string fileExtensionRegex = @"\.[a-z]+$";

        if (string.IsNullOrEmpty(extension) || extension.Length < 2 || extension.Length > 10 || !Regex.IsMatch(extension, fileExtensionRegex))
        {
            validationProblems.Add("extension",
                new[] {"Extension must be between 2 and 10 characters long and not empty. It must start with a dot and contain only lowercase letters."});
        }

        if (validationProblems.Any())
        {
            throw new ValidationException(EntityName.CodeLanguage, validationProblems);
        }
    }
}