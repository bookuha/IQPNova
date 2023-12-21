using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using CliWrap;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IQP.Infrastructure.CodeRunner;

public class CodeFileExecutorOptions
{
    public const string CodeFileExecutor = "CodeFileExecutor";
    public string SolutionFolderPath { get; set; } = null!;
    public string SamplesFolderPath { get; set; } = null!;
    public string RunnerScriptPath { get; set; } = null!;
}


public class CodeFileExecutor
{
    private readonly CodeFileExecutorOptions _options;
    private readonly ILogger<CodeFileExecutor> _logger;

    private static readonly IReadOnlyDictionary<CodeLanguage, string> CodeLanguageRunnerImages =
        new ReadOnlyDictionary<CodeLanguage, string>(new Dictionary<CodeLanguage, string>
        {
            [CodeLanguage.Csharp] = "exercism/csharp-test-runner",
            [CodeLanguage.Fsharp] = "exercism/fsharp-test-runner",
            [CodeLanguage.Java] = "exercism/java-test-runner",
        });
        

    public CodeFileExecutor(IOptions<CodeFileExecutorOptions> options, ILogger<CodeFileExecutor> logger)
    {
        _logger = logger;
        _options = options.Value;
    }
   
    
    public async Task<TestRun> ExecuteTestsOnSolution(string solutionCode, string testsCode, CodeLanguage codeLanguage, string username)
    {
        _logger.LogInformation(_options.SolutionFolderPath);
        _logger.LogInformation(_options.SamplesFolderPath);
        _logger.LogInformation(_options.RunnerScriptPath);
        
        var expectedSolutionPath = $"{_options.SolutionFolderPath}\\{GenerateSolutionName(username)}";
        
        try
        {
            var createdSolutionDir = await CreateSampleFiles(expectedSolutionPath, solutionCode, testsCode, codeLanguage);
            
            var createdSolutionPath = createdSolutionDir.FullName;
            
            await RunRunnerScript(createdSolutionPath, codeLanguage);
            var resultsJson = await ReadResultsJson(createdSolutionPath);

            Console.WriteLine(resultsJson);
            
            await CleanUpSolution(createdSolutionPath);

            return JsonSerializer.Deserialize<TestRun>(resultsJson)!;
        }
        catch (Exception)
        {
            await CleanUpSolution(expectedSolutionPath);
            throw;
        }
    }
    
    private async Task<DirectoryInfo> CreateSampleFiles(string dir, string solutionCode, string testsCode, CodeLanguage codeLanguage)
    {
        var samplesDir = new DirectoryInfo(@$"{_options.SamplesFolderPath}\{codeLanguage}"); // Add handling for no dirs

        var solutionDir = CopyAll(samplesDir.FullName, dir);
        await WriteCodeToFiles(solutionDir, solutionCode, testsCode);

        return solutionDir;
    }

    private async Task RenameSampleFiles(DirectoryInfo solutionDir, string customName)
    {
        
    }
    
    private static DirectoryInfo CopyAll(string sourceDir, string destDir, string? solutionName = "Solution")
    {
        var dir = new DirectoryInfo(sourceDir);
        
        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        var dirs = dir.GetDirectories();

        // Create the destination directory
        var resultDir = Directory.CreateDirectory(destDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        
        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(destDir, subDir.Name);
            CopyAll(subDir.FullName, newDestinationDir);
        }

        return resultDir;
    }

    private static async Task WriteCodeToFiles(DirectoryInfo solutionDir, string solutionCode, string testsCode)
    {
        if (!solutionDir.Exists)
        {
            throw new Exception("Got a problem creating files");
        }

        var solutionFilePath = solutionDir
            .GetFiles("*Solution.*", SearchOption.AllDirectories)
            .OrderBy(f => f.Name.Length)
            .First()
            .FullName; // This way we take .cs/.fs file, not .csproj/.fsproj. Will be remade later once needed.
        await AppendCodeToFile(solutionFilePath, solutionCode);
        
        var testsFilePath =
            solutionDir.GetFiles("*.*", SearchOption.AllDirectories)
                .Single(f => f.Name.Contains("Test", StringComparison.CurrentCultureIgnoreCase)).FullName;
        await AppendCodeToFile(testsFilePath, testsCode);
    }
    
    private static async Task AppendCodeToFile(string filePath, string code)
    {
        // Write code to the specified file
        await using var writer = new StreamWriter(filePath, true, Encoding.UTF8);
        await writer.WriteAsync(code);
    }

    private async Task<string> RunRunnerScript(string solutionPath, CodeLanguage codeLanguage)
    {
        var stdOut = new StringBuilder();
        var stdErr = new StringBuilder();

        var result = await Cli.Wrap("powershell")
            .WithArguments(args=> args
                .Add("Powershell -ExecutionPolicy Bypass")
                .Add(_options.RunnerScriptPath)
                .Add(CodeLanguageRunnerImages[codeLanguage])
                .Add("Solution") // This exact name is kind of standardized and class names better be of this name (Is a thing in fsharp/java runners at least)
                .Add(solutionPath) // : Input
                .Add(solutionPath)) // : Output (Results)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOut))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErr))
            .ExecuteAsync();

        Console.WriteLine(result.RunTime.Seconds);

        if (stdErr.Length > 0)
        {
            // TryRaiseSetupException(stdErr.ToString());
            Console.WriteLine(stdErr);
        }
        
        return stdOut.ToString();
    }
    
    private static async Task<string> ReadResultsJson(string solutionDir)
    {
        return await File.ReadAllTextAsync(solutionDir + "/results.json");
    }

    private string GenerateSolutionName(string username)
    {
        var solutionsFolder = new DirectoryInfo(_options.SolutionFolderPath);

        if (!solutionsFolder.Exists)
        {
            throw new Exception("You don't have solutions folder created");
        }
        
        var baseSolutionName = $"{username}_Solution";
        var solutionName = baseSolutionName;
        
        var counter = 1;
        while (SolutionNameExists(solutionsFolder, solutionName))
        {
            solutionName = $"{baseSolutionName}_{counter}";
            counter++;
        }

        return solutionName;
        
        static bool SolutionNameExists(DirectoryInfo solutionsFolder, string solutionName)
        {
            // Check if a directory with the given solution name exists
            return solutionsFolder.GetDirectories(solutionName).Any();
        }
        
    }
    
    private static async Task CleanUpSolution(string solutionPath)
    {
        var solutionFolder = new DirectoryInfo(solutionPath);

        if (!solutionFolder.Exists)
        {
            return;
        }
        
        solutionFolder.Delete(true);
    }

}