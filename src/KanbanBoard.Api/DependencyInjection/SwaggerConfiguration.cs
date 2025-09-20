using Microsoft.OpenApi.Models;

namespace KanbanBoard.Api.DependencyInjection;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(swaggerOptions =>
        {
            swaggerOptions.EnableAnnotations();
            swaggerOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "KanbanBoard API", Version = "v1" });
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(swaggerOptions =>
        {
            swaggerOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "KanbanBoard API V1");
            swaggerOptions.RoutePrefix = string.Empty;
        });

        return app;
    }
}
