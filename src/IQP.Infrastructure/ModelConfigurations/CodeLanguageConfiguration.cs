using IQP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class CodeLanguageConfiguration : IEntityTypeConfiguration<CodeLanguage>
{
    public void Configure(EntityTypeBuilder<CodeLanguage> builder)
    {
        builder
            .HasKey(l => l.Id);

        builder
            .Property(l => l.Name)
            .HasMaxLength(30)
            .IsRequired();
        
        builder
            .Property(l => l.Slug)
            .HasMaxLength(10)
            .IsRequired();

        builder
            .Property(l => l.Extension)
            .HasMaxLength(10)
            .IsRequired();

        // builder
        //    .HasMany<AlgoTaskCodeSnippet>(l => l.CodeSnippets)
        //    .WithOne(cs => cs.Language)                   Not needed as we never need to get all code snippets for a language
        //    .HasForeignKey(cs => cs.LanguageId)
        //    .IsRequired(); 
    }
    
}