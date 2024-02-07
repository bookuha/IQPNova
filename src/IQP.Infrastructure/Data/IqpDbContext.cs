using IQP.Domain.Entities;
using IQP.Infrastructure.ModelConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IQP.Infrastructure.Data;

public class IqpDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public IqpDbContext(DbContextOptions<IqpDbContext> options) : base(options)
    {
    }
    
    public DbSet<Question> Questions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Commentary> Commentaries { get; set; }
    public DbSet<AlgoTask> AlgoTasks { get; set; }
    public DbSet<AlgoTaskCategory> AlgoTaskCategories { get; set; }
    public DbSet<CodeLanguage> CodeLanguages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Changes AspNetUsers to simply Users
        modelBuilder.Entity<User>().ToTable("Users");
        
        modelBuilder.ApplyConfiguration(new QuestionConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new CommentaryConfiguration());
        modelBuilder.ApplyConfiguration(new TechTaskConfiguration());
        modelBuilder.ApplyConfiguration(new TechTaskSubmissionConfiguration());
        modelBuilder.ApplyConfiguration(new AlgoTaskConfiguration());
        modelBuilder.ApplyConfiguration(new AlgoTaskCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new CodeLanguageConfiguration());
        modelBuilder.ApplyConfiguration(new AlgoTaskCodeSnippetConfiguration());
    }
}