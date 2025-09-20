using KanbanBoard.Domain.Entities;
using KanbanBoard.Domain.Enums;

namespace KanbanBoard.Application.Interfaces;

public interface IBoardTaskService
{
    Task<IEnumerable<BoardTask>> GetAllAsync(Guid boardId, CancellationToken cancellationToken);
    Task<IEnumerable<BoardTask>> GetAllByStatusAsync(
        Guid boardId,
        BoardTaskStatus status,
        CancellationToken cancellationToken);
    Task<BoardTask> GetByIdAsync(Guid boardId, Guid id, CancellationToken cancellationToken);
    Task<BoardTask> CreateAsync(
        Guid boardId,
        BoardTask newBoardTask,
        string? idempotencyKey,
        CancellationToken cancellationToken);
    Task<BoardTask> UpdateAsync(Guid boardId, Guid id, BoardTask updatedBoardTask, CancellationToken cancellationToken);
    Task DeleteAsync(Guid boardId, Guid id, CancellationToken cancellationToken);
    Task<BoardTask> MoveAsync(
        Guid boardId,
        Guid id,
        BoardTaskStatus newStatus,
        Guid? previousTaskId,
        Guid? nextTaskId,
        CancellationToken cancellationToken);
    Task<IDictionary<BoardTaskStatus, IList<BoardTask>>> GetTasksGroupedByStatusAsync(
        Guid boardId,
        CancellationToken cancellationToken);

    IEnumerable<string> GetAllStatuses(Guid boardId);
}
