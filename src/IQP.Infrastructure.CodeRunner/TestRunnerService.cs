using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IQP.Infrastructure.CodeRunner;

public class TestRunnerOptions
{
    public const string TestRunner = "TestRunner";
    // Folder with sample files for different languages. Each language has its own folder.
    public string SamplesFolderPath { get; set; } = "/samples"; // Just a default value.
    // Folder with code files to be run and tested. Each solution has its own folder.
    public string SolutionsFolderPath { get; set; } = "/solutions"; // Just a default value.
}

/// <summary>
/// Class that runs provided tests on provided code in a Docker container.
/// </summary>
/// <remarks>
/// <para>
/// Code is run in a Docker container. Currently only DnD (Docker in Docker) is supported.
/// </para>
/// <para>
/// It requires Docker socket to be mounted to the host container and an external volume with "/solution" target.
/// </para>
/// <para>
/// Code is run in a container with an image that is specified by the language slug.
/// Code is put in a directory with sample files for the language that are previously provided by host machine,
/// then this folder is mounter to the "child" container (it is just a separate container created with Docker API)
/// and then code is run and tested. "Child" container writes results.json to the provided solution directory.
/// Results are then read from that file and TestRun object is returned. Solution folder is then deleted.
/// </para>
/// </remarks>
public class TestRunnerService : ITestRunnerService
{
    private readonly TestRunnerOptions _options;
    private readonly ILogger<TestRunnerService> _logger;
    private readonly ISlugToExecutorCodeLanguageConverter _slugToExecutorCodeLanguageConverter;
    private readonly IFileSolutionTestRunner _fileSolutionTestRunner;
    
    public TestRunnerService(IOptions<TestRunnerOptions> options, ILogger<TestRunnerService> logger,
        ISlugToExecutorCodeLanguageConverter slugToExecutorCodeLanguageConverter, IFileSolutionTestRunner fileSolutionTestRunner)
    {
        _options = options.Value;
        _logger = logger;
        _slugToExecutorCodeLanguageConverter = slugToExecutorCodeLanguageConverter;
        _fileSolutionTestRunner = fileSolutionTestRunner;
    }

    public Task<TestRun> RunTestsOnCode(string solutionCode, string testsCode, string languageSlug, string username)
    {
        var codeLanguage = _slugToExecutorCodeLanguageConverter.Convert(languageSlug);
        return RunTestsOnCode(solutionCode, testsCode, codeLanguage, username);
    }

    public async Task<TestRun> RunTestsOnCode(string solutionCode, string testsCode, ExecutorCodeLanguage codeLanguage,
        string username)
    {
        var solutionName = GenerateSolutionName(username);

        // This is by convention. "/solutions" is a volume mounted from Docker (For example in Compose file).
        var expectedSolutionPath = Path.Combine(_options.SolutionsFolderPath, solutionName);

        try
        {
            // Solution - just a convention for the name of the files you need to run the actual code in different languages.
            var createdSolutionDir =
                await CreateSolutionFiles(expectedSolutionPath, solutionCode, testsCode, codeLanguage);
            var result = await _fileSolutionTestRunner.RunTestsAsync(createdSolutionDir.FullName, codeLanguage);
            CleanUp(expectedSolutionPath);
            return result;
        }
        
        catch (Exception)
        {
            CleanUp(expectedSolutionPath);
            throw;
        }
    }

    private async Task<DirectoryInfo> CreateSolutionFiles(string dir, string solutionCode, string testsCode,
        ExecutorCodeLanguage codeLanguage)
    {
        // Sample files are stored in the app directory, unlike the solution files.
        var samplesDir =
            new DirectoryInfo(Path.Combine(_options.SamplesFolderPath,
                codeLanguage.ToString().ToLower())); // Add handling for no dirs

        if (!samplesDir.Exists)
            throw new SetupException(
                "Sample folder for this language was not found. Make sure to provide samples for all supported languages and have them named as languages names." +
                "Also, verify that @SamplesFolderPath is set correctly.");

        var solutionDir = CopyAll(samplesDir.FullName, dir);
        await WriteUserCodeToFiles(solutionDir, solutionCode, testsCode);

        return solutionDir;
    }

    private static DirectoryInfo CopyAll(string sourceDir, string destDir)
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

    private static async Task WriteUserCodeToFiles(DirectoryInfo solutionDir, string solutionCode, string testsCode)
    {
        if (!solutionDir.Exists)
            throw new SetupException(
                "Solution directory was not found. Make sure to create it before running the executor." +
                "Also, verify that @SolutionFolderPath is set correctly.");

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
    
    private static Task<string> ReadResultsJson(string solutionDir)
    {
        return File.ReadAllTextAsync(solutionDir + "/results.json");
    }

    private string GenerateSolutionName(string username)
    {
        var solutionsFolder = new DirectoryInfo(_options.SolutionsFolderPath);

        if (!solutionsFolder.Exists) throw new SetupException("You don't have solutions folder created");

        var baseSolutionName = $"{username}";
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

    private static void CleanUp(string solutionPath)
    {
        var solutionFolder = new DirectoryInfo(solutionPath);

        if (!solutionFolder.Exists) return;

        solutionFolder.Delete(true);
    }
}