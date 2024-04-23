using IQP.Infrastructure.CodeRunner;

namespace IQP.Web.Modules;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodeExecutionServices(this IServiceCollection services, IConfiguration config)
    {
        if(config["DOTNET_RUNNING_IN_CONTAINER"] == "true") // Will prepare solutions in Docker Volume and then run them In Docker container. Needed if host is itself running id Docker.
        {
            services.AddTransient<IFileSolutionTestRunner, DnDTestRunner>();
        }
        else // Will prepare solutions in local FS and then run them in Docker container.
        {
            services.AddTransient<IFileSolutionTestRunner, FileSystemToDockerTestRunner>();
        }
        
        services.AddTransient<ITestRunnerService, TestRunnerService>();
        services.AddTransient<ISlugToExecutorCodeLanguageConverter, SlugToExecutorCodeLanguageConverter>();
        services.AddOptions<TestRunnerOptions>()
            .Bind(config.GetSection(TestRunnerOptions.TestRunner));
        
        return services;
    }
}