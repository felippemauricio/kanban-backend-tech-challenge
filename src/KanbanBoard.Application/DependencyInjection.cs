using KanbanBoard.Application.Interfaces;
using KanbanBoard.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KanbanBoard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IBoardTaskService, BoardTaskService>();

        return services;
    }
}
