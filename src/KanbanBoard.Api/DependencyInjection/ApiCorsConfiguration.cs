using KanbanBoard.Config;

namespace KanbanBoard.Api.DependencyInjection;

public static class ApiCorsConfiguration
{
    public static IServiceCollection AddApiCorsConfiguration(this IServiceCollection services)
    {
        var corsOrigins = AppConfig.GetCorsOriginHost;

        if (!string.IsNullOrWhiteSpace(corsOrigins))
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Allow-Kanban-Board-Front", policy =>
                {
                    policy
                        .WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        return services;
    }

    public static WebApplication UseCorsPolicies(this WebApplication app)
    {
        app.UseCors("Allow-Kanban-Board-Front");

        return app;
    }
}
