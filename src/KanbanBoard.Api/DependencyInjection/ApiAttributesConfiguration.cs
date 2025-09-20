using KanbanBoard.Api.Middlewares;

namespace KanbanBoard.Api.DependencyInjection;


public static class ApiAttributesConfiguration
{
    public static IServiceCollection AddApiAttributesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ValidateBoardExistsAttribute>();

        return services;
    }
}

