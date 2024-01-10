using FluentValidation;
using IQP.Application.Services;
using IQP.Application.Services.Validators;

namespace IQP.Web.Modules;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<ICategoriesService, CategoriesService>();
        services.AddTransient<IAlgoTaskCategoriesService, AlgoTaskCategoriesService>();
        services.AddTransient<ICodeLanguagesService, CodeLanguagesService>();
        services.AddTransient<IQuestionsService, QuestionsService>();
        services.AddTransient<ICommentariesService, CommentariesService>();
        services.AddTransient<IAlgoTasksService, AlgoTasksService>();
        services.AddTransient<IUserService, UserService>();
        services.AddValidatorsFromAssemblyContaining<CreateCategoryCommandValidator>();
        
        return services;
    }
}