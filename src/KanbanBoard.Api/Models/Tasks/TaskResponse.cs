using System.Text.Json.Serialization;
using KanbanBoard.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace KanbanBoard.Api.Models.Tasks;

[SwaggerSchema("Task")]
public record TaskResponse(
    Guid Id,
    Guid BoardId,
    string Title,
    string? Description,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    BoardTaskStatus Status,
    string? LexoRankId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

