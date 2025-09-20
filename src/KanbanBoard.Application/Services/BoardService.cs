using KanbanBoard.Application.Exceptions;
using KanbanBoard.Application.Interfaces;

namespace KanbanBoard.Application.Services;

using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

public class BoardService(KanbanDbContext db) : IBoardService
{
    public async Task<IEnumerable<Board>> GetAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Board> boardList = await db.Boards.ToListAsync(cancellationToken);
        return boardList;
    }

    public async Task<Board> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Board? board = await db.Boards.FindAsync([id], cancellationToken);
        return board ?? throw new BoardNotFoundException(id);
    }

    private async Task<Board?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken)
    {
        Board? board = await db.Boards.FirstOrDefaultAsync(
            board => board.IdempotencyKey == idempotencyKey,
            cancellationToken);

        return board;
    }

    public async Task<Board> CreateAsync(Board newBoard, string? idempotencyKey, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            Board? board = await GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
            if (board != null)
            {
                return board;
            }

            newBoard.SetIdempotencyKey(idempotencyKey);
        }

        db.Boards.Add(newBoard);
        await db.SaveChangesAsync(cancellationToken);

        return newBoard;
    }

    public async Task<Board> UpdateAsync(Guid id, Board updatedBoard, CancellationToken cancellationToken)
    {
        Board board = await GetByIdAsync(id, cancellationToken);

        board.Update(updatedBoard.Name);

        await db.SaveChangesAsync(cancellationToken);
        return board;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Board board = await GetByIdAsync(id, cancellationToken);

        db.Boards.Remove(board);
        await db.SaveChangesAsync(cancellationToken);
    }
}

