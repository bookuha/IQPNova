using IQP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IQP.Infrastructure.ModelConfigurations;

public class AlgoTaskCategoryConfiguration : IEntityTypeConfiguration<AlgoTaskCategory>
{
    public void Configure(EntityTypeBuilder<AlgoTaskCategory> builder)
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
            .HasMany<AlgoTask>(c => c.AlgoTasks)
            .WithOne(q => q.AlgoCategory)
            .HasForeignKey(q => q.AlgoCategoryId)
            .IsRequired();
    }
    
}