using FluentValidation;
using IQP.Application.Services.Users;
using IQP.Application.Usecases.Categories.Create;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Repositories;

namespace IQP.Web.Modules;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IAlgoCategoriesRepository, AlgoCategoriesRepository>();
        services.AddTransient<ICategoriesRepository, CategoriesRepository>();
        services.AddTransient<ICodeLanguagesRepository, CodeLanguagesRepository>();
        services.AddTransient<IAlgoTasksRepository, AlgoTaskRepository>();
        services.AddTransient<IQuestionsRepository, QuestionsRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IAlgoTasksRepository, AlgoTaskRepository>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCategoryCommand>());
        services.AddValidatorsFromAssemblyContaining<CreateCategoryCommandValidator>();
        
        return services;
    }
}