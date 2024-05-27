using IQP.Domain.Entities;
using IQP.Domain.Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class TechTaskConfiguration : IEntityTypeConfiguration<TechTask>
{
    public void Configure(EntityTypeBuilder<TechTask> builder)
    {
        builder
            .HasKey(t => t.Id);
        
        builder
            .Property(t=>t.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property<string>(t => t.Title)
            .HasMaxLength(100)
            .IsRequired();
        
        builder
            .Property<string>(t => t.Description)
            .HasMaxLength(750)
            .IsRequired();

        builder
            .Property(t => t.ActiveUntil)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .HasOne<User>(t => t.Creator)
            .WithMany(u => u.CreatedTechTasks)
            .HasForeignKey(t => t.CreatorId)
            .IsRequired();

        builder
            .HasOne<Category>(t => t.Category)
            .WithMany() // We can declare TechTasks[] in Category and it will not affect the actual DB table structure, but I won't do this for now
            .HasForeignKey(t => t.CategoryId)
            .IsRequired();

        builder
            .HasMany<TechTaskSubmission>(t => t.Submissions)
            .WithOne(s => s.TechTask)
            .HasForeignKey(s => s.TechTaskId)
            .IsRequired();
    }
}