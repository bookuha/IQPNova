using IQP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class CommentaryConfiguration : IEntityTypeConfiguration<Commentary>
{
    public void Configure(EntityTypeBuilder<Commentary> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property<string>(c => c.Content)
            .HasMaxLength(300)
            .IsRequired();

        builder
            .HasMany<Commentary>(c => c.Replies)
            .WithOne(c => c.ReplyTo)
            .HasForeignKey(c => c.ReplyToId);
        
        builder
            .HasMany(c => c.LikedBy)
            .WithMany(u => u.LikedCommentaries);

        builder
            .HasOne<Question>(c => c.Question)
            .WithMany(q => q.Commentaries)
            .HasForeignKey(c => c.QuestionId)
            .IsRequired();

        builder
            .HasOne<User>(c => c.CreatedBy)
            .WithMany(u => u.CreatedCommentaries)
            .HasForeignKey(c => c.CreatedById)
            .IsRequired();
    }
}