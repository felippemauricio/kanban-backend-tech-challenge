using KanbanBoard.Api.Models.Tasks;
using KanbanBoard.Domain.Entities;

namespace KanbanBoard.Api.Mappers;

public static class TaskMapper
{
    public static TaskResponse ToResponse(this BoardTask boardTask) =>
        new(
            boardTask.Id,
            boardTask.BoardId,
            boardTask.Title,
            boardTask.Description,
            boardTask.Status,
            boardTask.LexoRankId,
            boardTask.CreatedAt,
            boardTask.UpdatedAt
        );

    public static BoardTask ToDomain(this TaskRequest taskRequest, Guid boardId) =>
        new(
            boardId,
            taskRequest.Title
        )
        {
            Description = taskRequest.Description,
        };
}
