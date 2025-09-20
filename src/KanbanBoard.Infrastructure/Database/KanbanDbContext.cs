using KanbanBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Infrastructure.Database;

public class KanbanDbContext(DbContextOptions<KanbanDbContext> options) : DbContext(options)
{
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardTask> BoardTasks => Set<BoardTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Board>()
            .HasIndex(board => board.IdempotencyKey)
            .IsUnique();

        modelBuilder.Entity<Board>()
            .Property(board => board.CreatedAt)
            .HasConversion(
                dateTime => dateTime,
                dateTime => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            );

        modelBuilder.Entity<Board>()
            .Property(board => board.UpdatedAt)
            .HasConversion(
                dateTime => dateTime,
                dateTime => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            );

        modelBuilder.Entity<BoardTask>()
            .HasIndex(boardTask => boardTask.IdempotencyKey)
            .IsUnique();

        modelBuilder.Entity<BoardTask>()
            .HasIndex(boardTask => new { boardTask.Status, boardTask.LexoRankId });

        modelBuilder.Entity<BoardTask>()
            .Property(boardTask => boardTask.CreatedAt)
            .HasConversion(
                dateTime => dateTime,
                dateTime => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            );

        modelBuilder.Entity<BoardTask>()
            .Property(boardTask => boardTask.UpdatedAt)
            .HasConversion(
                dateTime => dateTime,
                dateTime => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            );
    }
}
