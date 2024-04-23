using System.Text.Json;
using System.Text.Json.Serialization;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace IQP.Infrastructure.CodeRunner;

public class FileSystemToDockerTestRunner : IFileSolutionTestRunner
{
    private static readonly Dictionary<ExecutorCodeLanguage, string> CodeLanguageRunnerImages = new()
    {
        [ExecutorCodeLanguage.Csharp] = "exercism/csharp-test-runner",
        [ExecutorCodeLanguage.Fsharp] = "exercism/fsharp-test-runner",
        [ExecutorCodeLanguage.Java] = "exercism/java-test-runner"
    };
    
    private readonly ILogger<FileSystemToDockerTestRunner> _logger;

    public FileSystemToDockerTestRunner(ILogger<FileSystemToDockerTestRunner> logger)
    {
        _logger = logger;
    }

    public async Task<TestRun> RunTestsAsync(string solutionPath, ExecutorCodeLanguage language)
    {
        var client = new DockerClientConfiguration().CreateClient();

        var container = await client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = CodeLanguageRunnerImages[language],
            Cmd = new List<string> {"Solution", "/solution", "/solution"},
            HostConfig = new HostConfig
            {
                NetworkMode = "none",
                Binds = new List<string> {solutionPath + ":/solution"}
            }
        });

        await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

        var result = await client.Containers.WaitContainerAsync(container.ID);

        var logStream = await client.Containers.GetContainerLogsAsync(container.ID, new ContainerLogsParameters
        {
            ShowStdout = true,
            ShowStderr = true
        });

        using var reader = new StreamReader(logStream);
        var log = await reader.ReadToEndAsync();

        _logger.LogInformation("Code running Container log: {Log}", log);
        
        
        var resultsJson = await File.ReadAllTextAsync(solutionPath + "/results.json");
        
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        return JsonSerializer.Deserialize<TestRun>(resultsJson, options)!;
    }
}