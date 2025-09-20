using Swashbuckle.AspNetCore.Annotations;

namespace KanbanBoard.Api.Models.Boards;

[SwaggerSchema("Board")]
public record BoardResponse(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
