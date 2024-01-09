using IQP.Infrastructure.CodeRunner;

namespace IQP.Web.Modules;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodeExecutionServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<ITestRunner, CodeFileExecutor>();
        services.AddTransient<ISlugToExecutorCodeLanguageConverter, SlugToExecutorCodeLanguageConverter>();
        services.AddOptions<CodeFileExecutorOptions>()
            .Bind(config.GetSection(CodeFileExecutorOptions.CodeFileExecutor));
        
        return services;
    }
}