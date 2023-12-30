namespace IQP.Domain.Entities;

public class CodeLanguage
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Extension { get; set; }
}