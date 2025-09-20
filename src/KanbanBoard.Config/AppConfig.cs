using Microsoft.Extensions.Configuration;

namespace KanbanBoard.Config;


public static class AppConfig
{
    private static IConfiguration? _configuration;

    public static void Init(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string DbFileName =>
        _configuration?["ConnectionStrings:DefaultConnection"] ?? "kanban-board.db";

    public static string DbFolder =>
        _configuration?["ConnectionStrings:DefaultFolder"] ?? "data";

    public static string Environment =>
        System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    public static string GetCorsOriginHost =>
        System.Environment.GetEnvironmentVariable("CORS_ORIGIN_HOST") ?? "http://localhost:5173";

    public static bool IsDevelopment =>
        Environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
}
