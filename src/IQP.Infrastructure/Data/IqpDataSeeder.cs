using IQP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IQP.Infrastructure.Data;

public class IqpDataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var csharpId = Guid.NewGuid();
        var fsharpId = Guid.NewGuid();
        var javaId = Guid.NewGuid();

        modelBuilder.Entity<CodeLanguage>().HasData(new List<CodeLanguage>
            {
                new() {Id = csharpId, Name = "C#", Slug = "csharp", Extension = ".cs"},
                new() {Id = fsharpId, Name = "F#", Slug = "fsharp", Extension = ".fs"},
                new() {Id = javaId, Name = "Java", Slug = "java", Extension = ".java"}
            }
        );
        
        var algoTaskCategoryId = Guid.NewGuid();
        modelBuilder.Entity<AlgoTaskCategory>().HasData(new AlgoTaskCategory
        {
            Id = algoTaskCategoryId,
            Title = "Entry level",
            Description = "Entry level tasks for beginners"
        });

        
        var algoTaskId = Guid.NewGuid();
        modelBuilder.Entity<AlgoTask>().HasData(new AlgoTask
        {
            Id = algoTaskId,
            Title = "Hello, World!",
            Description = "Return Hello World!, fighter!",
            AlgoCategoryId = algoTaskCategoryId,
            
        });

        modelBuilder.Entity<AlgoTaskCodeSnippet>().HasData(new AlgoTaskCodeSnippet
        {
            Id = Guid.NewGuid(),
            AlgoTaskId = algoTaskId,
            LanguageId = csharpId,
            SampleCode =
                """public class Solution { public static string Solution {return "Hello, World!"} } }""",
            TestsCode =
                """public class SolutionTests { [ Fact ] public void TestHelloWorld ( ) { Assert.Equal ("Hello, World!" , Solution()); } }"""
        });
    }
}