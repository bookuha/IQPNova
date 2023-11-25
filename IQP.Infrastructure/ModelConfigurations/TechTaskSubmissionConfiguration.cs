using IQP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class TechTaskSubmissionConfiguration : IEntityTypeConfiguration<TechTaskSubmission>
{
    public void Configure(EntityTypeBuilder<TechTaskSubmission> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property<string>(s => s.Content)
            .HasMaxLength(900)
            .IsRequired();

        builder
            .Property<string>(s => s.Review)
            .HasMaxLength(900)
            .IsRequired(false);

        builder
            .HasOne<TechTask>(s => s.TechTask)
            .WithMany(t => t.Submissions)
            .HasForeignKey(s => s.TechTaskId)
            .IsRequired();

        builder
            .HasOne<User>(t => t.Creator)
            .WithMany(u => u.TechTaskSubmissions)
            .HasForeignKey(t => t.CreatorId)
            .IsRequired();
    }
}