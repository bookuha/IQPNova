using IQP.Domain.Entities;
using IQP.Domain.Entities.AlgoTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class AlgoTaskCodeSnippetConfiguration : IEntityTypeConfiguration<AlgoTaskCodeSnippet>
{
    public void Configure(EntityTypeBuilder<AlgoTaskCodeSnippet> builder)
    {
        builder
            .HasKey(cs => cs.Id);

        builder
            .Property(cs => cs.SampleCode)
            .HasMaxLength(1000)
            .IsRequired();

        builder
            .Property(cs => cs.TestsCode)
            .HasMaxLength(3000)
            .IsRequired();
        
        builder
            .HasOne<AlgoTask>(cs => cs.AlgoTask)
            .WithMany(t => t.CodeSnippets)
            .HasForeignKey(cs => cs.AlgoTaskId)
            .IsRequired();

        builder
            .HasOne<CodeLanguage>(cs => cs.Language)
            .WithMany()
            .HasForeignKey(cs => cs.LanguageId)
            .IsRequired();
    }
    
}