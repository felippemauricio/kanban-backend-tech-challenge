using KanbanBoard.Application.Exceptions;
using KanbanBoard.Application.Interfaces;
using KanbanBoard.Application.Utils;
using KanbanBoard.Domain.Entities;
using KanbanBoard.Domain.Enums;
using KanbanBoard.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Application.Services;

public class BoardTaskService(KanbanDbContext db) : IBoardTaskService
{
    public async Task<IEnumerable<BoardTask>> GetAllAsync(Guid boardId, CancellationToken cancellationToken)
    {
        IEnumerable<BoardTask> taskList =
            await db.BoardTasks
                .Where(boardTask => boardTask.BoardId == boardId)
                .OrderBy(boardTask => boardTask.Status)
                .ThenBy(boardTask => boardTask.LexoRankId)
                .ToListAsync(cancellationToken);

        return taskList;
    }

    public async Task<IEnumerable<BoardTask>> GetAllByStatusAsync(
        Guid boardId,
        BoardTaskStatus status,
        CancellationToken cancellationToken)
    {
        IEnumerable<BoardTask> taskList = await db.BoardTasks
            .Where(boardTask => boardTask.BoardId == boardId && boardTask.Status == status)
            .OrderBy(boardTask => boardTask.LexoRankId)
            .ToListAsync(cancellationToken);

        return taskList;
    }

    public async Task<BoardTask> GetByIdAsync(Guid boardId, Guid id, CancellationToken cancellationToken)
    {
        BoardTask? boardTask = await db.BoardTasks
            .FirstOrDefaultAsync(boardTask => boardTask.BoardId == boardId && boardTask.Id == id, cancellationToken);
        return boardTask ?? throw new BoardTaskNotFoundException(id);
    }

    private async Task<BoardTask?> GetByIdempotencyKeyAsync(
        Guid boardId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        BoardTask? boardTask = await db.BoardTasks.FirstOrDefaultAsync(
            boardTask => boardTask.BoardId == boardId && boardTask.IdempotencyKey == idempotencyKey,
            cancellationToken);

        return boardTask;
    }

    private async Task<BoardTask?> GetLatestTaskAsync(
        Guid boardId,
        BoardTaskStatus status,
        CancellationToken cancellationToken)
    {
        BoardTask? boardTask = await db.BoardTasks
            .Where(boardTask => boardTask.BoardId == boardId && boardTask.Status == status)
            .OrderByDescending(boardTask => boardTask.LexoRankId)
            .FirstOrDefaultAsync(cancellationToken);

        return boardTask;
    }

    public async Task<BoardTask> CreateAsync(
        Guid boardId,
        BoardTask newBoardTask,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            BoardTask? boardTask = await GetByIdempotencyKeyAsync(boardId, idempotencyKey, cancellationToken);
            if (boardTask != null)
            {
                return boardTask;
            }

            newBoardTask.SetIdempotencyKey(idempotencyKey);
        }

        if (string.IsNullOrWhiteSpace(newBoardTask.LexoRankId))
        {
            BoardTask? boardTask = await GetLatestTaskAsync(boardId, newBoardTask.Status, cancellationToken);
            newBoardTask.SetLexoRankId(LexoRankUtil.GetLexoRankIdForCreation(boardTask?.LexoRankId));
        }

        db.BoardTasks.Add(newBoardTask);
        await db.SaveChangesAsync(cancellationToken);

        return newBoardTask;
    }

    public async Task<BoardTask> UpdateAsync(
        Guid boardId,
        Guid id,
        BoardTask updatedBoardTask,
        CancellationToken cancellationToken)
    {
        BoardTask boardTask = await GetByIdAsync(boardId, id, cancellationToken);

        boardTask.Update(updatedBoardTask.Title, updatedBoardTask.Description);

        await db.SaveChangesAsync(cancellationToken);
        return boardTask;
    }

    public async Task DeleteAsync(Guid boardId, Guid id, CancellationToken cancellationToken)
    {
        BoardTask boardTask = await GetByIdAsync(boardId, id, cancellationToken);

        db.BoardTasks.Remove(boardTask);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<BoardTask> MoveAsync(
        Guid boardId,
        Guid id,
        BoardTaskStatus newStatus,
        Guid? previousTaskId,
        Guid? nextTaskId,
        CancellationToken cancellationToken)
    {
        BoardTask boardTask = await GetByIdAsync(boardId, id, cancellationToken);

        string? prevLexoRankId = null;
        string? nextLexoRankId = null;

        if (previousTaskId.HasValue)
        {
            BoardTask prevTask = await GetByIdAsync(boardId, id: previousTaskId.Value, cancellationToken);
            prevLexoRankId = prevTask.LexoRankId;
        }

        if (nextTaskId.HasValue)
        {
            BoardTask nextTask = await GetByIdAsync(boardId, id: nextTaskId.Value, cancellationToken);
            nextLexoRankId = nextTask.LexoRankId;
        }

        var newRankId = LexoRankUtil.GetLexoRankIdForMove(prevLexoRankId, nextLexoRankId);
        boardTask.Move(newStatus, newRankId);

        await db.SaveChangesAsync(cancellationToken);
        return boardTask;
    }

    public async Task<IDictionary<BoardTaskStatus, IList<BoardTask>>> GetTasksGroupedByStatusAsync(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        IEnumerable<BoardTask> taskList = await GetAllAsync(boardId, cancellationToken);

        var grouped = Enum.GetValues<BoardTaskStatus>()
            .ToDictionary(
                status => status,
                IList<BoardTask> (_) => new List<BoardTask>()
            );

        foreach (BoardTask task in taskList)
        {
            grouped[task.Status].Add(task);
        }

        return grouped;
    }

    public IEnumerable<string> GetAllStatuses(Guid boardId)
    {
        return Enum.GetNames<BoardTaskStatus>().ToList();
    }
}
