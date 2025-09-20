using KanbanBoard.Domain.Entities;

namespace KanbanBoard.Application.Interfaces;

public interface IBoardService
{
    Task<IEnumerable<Board>> GetAllAsync(CancellationToken cancellationToken);
    Task<Board> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Board> CreateAsync(Board newBoard, string? idempotencyKey, CancellationToken cancellationToken);
    Task<Board> UpdateAsync(Guid id, Board updatedBoard, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
