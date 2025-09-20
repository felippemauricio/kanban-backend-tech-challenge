using KanbanBoard.Api.Models.Boards;
using KanbanBoard.Domain.Entities;

namespace KanbanBoard.Api.Mappers;

public static class BoardMapper
{
    public static BoardResponse ToResponse(this Board board) =>
        new(
            board.Id,
            board.Name,
            board.CreatedAt,
            board.UpdatedAt
        );

    public static Board ToDomain(this BoardRequest boardRequest) =>
        new(
            boardRequest.Name
        );
}
