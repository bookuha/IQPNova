using IQP.Domain.Entities;
using IQP.Domain.Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property<string>(c => c.Title)
            .HasMaxLength(30)
            .IsRequired();
        
        
        builder
            .Property<string>(c => c.Description)
            .HasMaxLength(120)
            .IsRequired();

        builder
            .HasMany<Question>(c => c.Questions)
            .WithOne(q => q.Category)
            .HasForeignKey(q => q.CategoryId)
            .IsRequired();
    }
}