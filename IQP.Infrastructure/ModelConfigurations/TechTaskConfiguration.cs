using IQP.Domain.Entities;
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
            .Property<string>(t => t.Title)
            .HasMaxLength(30)
            .IsRequired();
        
        builder
            .Property<string>(t => t.Description)
            .HasMaxLength(320)
            .IsRequired();

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