using IQP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder
            .HasKey(q => q.Id);

        builder
            .Property<string>(q => q.Title)
            .HasMaxLength(30)
            .IsRequired();

        builder
            .Property<string>(q => q.Description)
            .HasMaxLength(120)
            .IsRequired();

        builder
            .HasOne<Category>(q => q.Category)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CategoryId)
            .IsRequired();

        builder
            .HasOne<User>(q => q.Creator)
            .WithMany(u=>u.CreatedQuestions)
            .HasForeignKey(q => q.CreatorId)
            .IsRequired();

        builder
            .HasMany(q => q.LikedBy)
            .WithMany(u => u.LikedQuestions);

        builder
            .HasMany<Commentary>(q => q.Commentaries)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.QuestionId)
            .IsRequired();
    }
}