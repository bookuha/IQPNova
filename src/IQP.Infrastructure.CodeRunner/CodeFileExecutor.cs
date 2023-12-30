using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CliWrap;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IQP.Infrastructure.CodeRunner;

public class CodeFileExecutorOptions
{
    public const string CodeFileExecutor = "CodeRunner";
    public string SolutionFolderPath { get; set; } = null!;
    public string SamplesFolderPath { get; set; } = null!;
    public string RunnerScriptPath { get; set; } = null!;
}


public class CodeFileExecutor : ITestRunner
{
    private readonly CodeFileExecutorOptions _options;
    private readonly ILogger<CodeFileExecutor> _logger;
    private readonly ISlugToExecutorCodeLanguageConverter _slugToExecutorCodeLanguageConverter;

    private static readonly IReadOnlyDictionary<ExecutorCodeLanguage, string> CodeLanguageRunnerImages = // TODO: Introduce a new class for getting image by language
        new ReadOnlyDictionary<ExecutorCodeLanguage, string>(new Dictionary<ExecutorCodeLanguage, string>
        {
            [ExecutorCodeLanguage.Csharp] = "exercism/csharp-test-runner",
            [ExecutorCodeLanguage.Fsharp] = "exercism/fsharp-test-runner",
            [ExecutorCodeLanguage.Java] = "exercism/java-test-runner",
        });
        

    public CodeFileExecutor(IOptions<CodeFileExecutorOptions> options, ILogger<CodeFileExecutor> logger, ISlugToExecutorCodeLanguageConverter slugToExecutorCodeLanguageConverter)
    {
        _options = options.Value;
        _logger = logger;
        _slugToExecutorCodeLanguageConverter = slugToExecutorCodeLanguageConverter;
    }
   
    
    public Task<TestRun> ExecuteTestsOnCode(string solutionCode, string testsCode, string languageSlug, string username)
    {
        var codeLanguage = _slugToExecutorCodeLanguageConverter.Convert(languageSlug);
        return ExecuteTestsOnCode(solutionCode, testsCode, codeLanguage, username);
    }
    
    public async Task<TestRun> ExecuteTestsOnCode(string solutionCode, string testsCode, ExecutorCodeLanguage codeLanguage, string username)
    {
        var expectedSolutionPath = $"{_options.SolutionFolderPath}\\{GenerateSolutionName(username)}";
        
        try
        {
            var createdSolutionDir = await CreateSampleFiles(expectedSolutionPath, solutionCode, testsCode, codeLanguage);
            
            var createdSolutionPath = createdSolutionDir.FullName;
            
            await RunRunnerScript(createdSolutionPath, codeLanguage);
            var resultsJson = await ReadResultsJson(createdSolutionPath);

            Console.WriteLine(resultsJson);
            
            await CleanUpSolution(createdSolutionPath);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            
            return JsonSerializer.Deserialize<TestRun>(resultsJson, options)!;
        }
        catch (Exception)
        {
            await CleanUpSolution(expectedSolutionPath);
            throw;
        }
    }
    
    private static ExecutorCodeLanguage GetCodeLanguageFromSlug(string languageSlug)
    {
        return languageSlug switch
        {
            "csharp" => ExecutorCodeLanguage.Csharp,
            "fsharp" => ExecutorCodeLanguage.Fsharp,
            "java" => ExecutorCodeLanguage.Java,
            _ => throw new ArgumentException("Language slug is not supported. Make sure you didn't make a typo." +
                                             "Supported languages are: csharp, fsharp, java", nameof(languageSlug))
        };
    }
    
    private async Task<DirectoryInfo> CreateSampleFiles(string dir, string solutionCode, string testsCode, ExecutorCodeLanguage codeLanguage)
    {
        var samplesDir = new DirectoryInfo(@$"{_options.SamplesFolderPath}\{codeLanguage}"); // Add handling for no dirs
        
        if (!samplesDir.Exists)
        {
            throw new SetupException(
                "Sample folder for this language was not found. Make sure to provide samples for all supported languages and have them named as languages names." +
                "Also, verify that @SamplesFolderPath is set correctly.");
        }

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
            throw new SetupException("Solution directory was not found. Make sure to create it before running the executor." +
                                     "Also, verify that @SolutionFolderPath is set correctly.");
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

    private async Task<string> RunRunnerScript(string solutionPath, ExecutorCodeLanguage codeLanguage)
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
            if (stdErr.ToString().StartsWith("docker: error during connect: this error may indicate that the docker daemon is not running"))
            {
                throw new SetupException("Docker is not running. Make sure to start it before running the executor.");
            }
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