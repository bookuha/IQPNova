using IQP.Infrastructure.CodeRunner;

namespace IQP.Web.Modules;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodeExecutionServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<ITestRunner, DockerTestRunner>();
        services.AddTransient<ISlugToExecutorCodeLanguageConverter, SlugToExecutorCodeLanguageConverter>();
        services.AddOptions<DockerTestRunnerOptions>()
            .Bind(config.GetSection(DockerTestRunnerOptions.TestRunner));
        
        return services;
    }
}