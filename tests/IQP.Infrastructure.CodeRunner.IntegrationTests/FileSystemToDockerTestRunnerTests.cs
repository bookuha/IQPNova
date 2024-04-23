using Microsoft.Extensions.Logging;
using Moq;

namespace IQP.Infrastructure.CodeRunner.IntegrationTests;

[TestFixture]
public class FileSystemToDockerTestRunnerTests
{
    private readonly IFileSolutionTestRunner _fileSolutionTestRunner = new FileSystemToDockerTestRunner(new Mock<ILogger<FileSystemToDockerTestRunner>>().Object);

    [Test]
    public async Task RunTestsAsync_Should_Run_CSharp_Successfully()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\csharp\hello-world");
        
        var language = ExecutorCodeLanguage.Csharp;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Pass));
    }
    
    [Test]
    public async Task RunTestsAsync_Should_Run_CSharp_Fail()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\csharp\hello-world-fail");
        
        var language = ExecutorCodeLanguage.Csharp;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));
    }
    
    [Test]
    public async Task RunTestsAsync_Should_Run_CSharp_CompileError()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\csharp\hello-world-compile-error");
        
        var language = ExecutorCodeLanguage.Csharp;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Error));
    }
    
    [Test]
    public async Task RunTestsAsync_Should_Run_FSharp_Successfully()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\fsharp\hello-world");
        
        var language = ExecutorCodeLanguage.Fsharp;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Pass));
    }
    
    [Test]
    public async Task RunTestsAsync_Should_Run_FSharp_Fail()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\fsharp\hello-world-fail");
        
        var language = ExecutorCodeLanguage.Fsharp;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));
    }
    
    [Test]
    public async Task RunTestsAsync_Should_Run_FSharp_CompileError()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\fsharp\hello-world-compile-error");
        
        var language = ExecutorCodeLanguage.Fsharp;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Error));
    }
    
    [Test]
    public async Task RunTestsAsync_Should_Run_Java_Successfully()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\java\hello-world");
        
        var language = ExecutorCodeLanguage.Java;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Pass));
    }
    
    [Test]
    public async Task RunTestsAsync_Should_Run_Java_Fail()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\java\hello-world-fail");
        
        var language = ExecutorCodeLanguage.Java;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Fail));
    }
    
    [Test]
    public async Task RunTestsAsync_Should_Run_Java_CompileError()
    {
        // Arrange
        var solutionDirectory = new DirectoryInfo(@"C:\Users\booku\RiderProjects\IQPNova\solutions\integrationTestsSolutions\java\hello-world-compile-error");
        
        var language = ExecutorCodeLanguage.Java;
        
        // Act
        var result = await _fileSolutionTestRunner.RunTestsAsync(solutionDirectory.FullName, language);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(TestStatus.Error));
    }
    
    
}