using IQP.Domain.Entities;
using IQP.Infrastructure.ModelConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IQP.Infrastructure.Data;

public class IqpDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public IqpDbContext()
    {
    }
    
    public IqpDbContext(DbContextOptions<IqpDbContext> options) : base(options)
    {
    }
    
    // DbSets are virtual to allow mocking in unit tests
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Commentary> Commentaries { get; set; }
    public virtual DbSet<AlgoTask> AlgoTasks { get; set; }
    public virtual DbSet<AlgoTaskCategory> AlgoTaskCategories { get; set; }
    public virtual DbSet<CodeLanguage> CodeLanguages { get; set; }

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
        // Seed code languages, algo task category and a simple algo task
        IqpDataSeeder.Seed(modelBuilder);
    }
}