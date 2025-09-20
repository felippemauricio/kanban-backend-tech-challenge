using KanbanBoard.Config;
using KanbanBoard.Infrastructure.Database.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KanbanBoard.Infrastructure.Database;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDatabase(this IServiceCollection services)
    {
        string? rootFolder;
        if (AppConfig.IsDevelopment)
        {
            rootFolder = Directory.GetParent(
                Directory.GetParent(Directory.GetCurrentDirectory())?.FullName
                ?? Directory.GetCurrentDirectory()
            )?.FullName;
        }
        else
        {
            rootFolder = Directory.GetCurrentDirectory();
        }

        var dataFolder = Path.Combine(rootFolder!, AppConfig.DbFolder);
        Console.WriteLine($"[Database] Using folder: {dataFolder}");

        if (!Directory.Exists(dataFolder))
        {
            Console.WriteLine($"[Database] Creating database folder at: {dataFolder}");
            Directory.CreateDirectory(dataFolder);
        }

        // Database path
        var dbPath = Path.Combine(dataFolder, AppConfig.DbFileName);
        Console.WriteLine(File.Exists(dbPath)
            ? $"[Database] Using existing database file: {dbPath}"
            : $"[Database] Creating database file: {dbPath}");

        services.AddDbContext<KanbanDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        KanbanDbContext dbContext = scope.ServiceProvider.GetRequiredService<KanbanDbContext>();

        await dbContext.Database.EnsureCreatedAsync();

        await BoardSeed.Initialize(dbContext);
        await BoardTasksSeed.Initialize(dbContext);
    }
}
