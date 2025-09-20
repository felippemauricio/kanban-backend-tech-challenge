using KanbanBoard.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace KanbanBoard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddInfrastructureDatabase();
        return services;
    }
}
