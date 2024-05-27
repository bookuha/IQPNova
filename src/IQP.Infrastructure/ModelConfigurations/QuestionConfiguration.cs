using IQP.Domain.Entities;
using IQP.Domain.Entities.Questions;
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
            .Property(q => q.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property<string>(q => q.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property<string>(q => q.Description)
            .HasMaxLength(600)
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
            .WithMany(u => u.LikedQuestions)
            .UsingEntity("UsersLikedQuestions");

        builder
            .HasMany<Commentary>(q => q.Commentaries)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.QuestionId)
            .IsRequired();
    }
}