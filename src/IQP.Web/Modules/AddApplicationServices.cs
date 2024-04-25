using FluentValidation;
using IQP.Application.Services.Users;
using IQP.Application.Usecases.Categories.Create;

namespace IQP.Web.Modules;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCategoryCommand>());
        services.AddValidatorsFromAssemblyContaining<CreateCategoryCommandValidator>();
        
        return services;
    }
}