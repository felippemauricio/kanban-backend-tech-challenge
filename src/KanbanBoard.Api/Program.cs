using KanbanBoard.Api.DependencyInjection;
using KanbanBoard.Application;
using KanbanBoard.Config;
using KanbanBoard.Infrastructure;
using KanbanBoard.Infrastructure.Database;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Config - AppSettings
AppConfig.Init(builder.Configuration);

builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddApiAttributesConfiguration();
builder.Services.AddApiConfiguration();
builder.Services.AddApiCorsConfiguration();
builder.Services.AddSwaggerConfiguration();


WebApplication app = builder.Build();

// Initialize DB
await app.Services.InitializeDatabaseAsync();
app.UseCorsPolicies();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseSwaggerDocumentation();

app.Run();
