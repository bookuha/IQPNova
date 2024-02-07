using IQP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class AlgoTaskConfiguration : IEntityTypeConfiguration<AlgoTask>
{
    public void Configure(EntityTypeBuilder<AlgoTask> builder)
    {
        builder
            .Property(t => t.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property<string>(t => t.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property<string>(t => t.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder
            .HasOne<AlgoTaskCategory>(t => t.AlgoCategory)
            .WithMany(c => c.AlgoTasks)
            .HasForeignKey(t => t.AlgoCategoryId)
            .IsRequired();
        
        builder
            .HasMany<AlgoTaskCodeSnippet>(t => t.CodeSnippets)
            .WithOne(cs => cs.AlgoTask)
            .HasForeignKey(cs => cs.AlgoTaskId)
            .IsRequired();

        builder
            .HasMany(t => t.PassedBy)
            .WithMany(u => u.PassedAlgoTasks)
            .UsingEntity("UsersPassedAlgoTasks");
    }
}