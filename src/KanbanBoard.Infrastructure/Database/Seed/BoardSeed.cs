using KanbanBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Infrastructure.Database.Seed;

public static class BoardSeed
{
    public static async Task Initialize(KanbanDbContext context)
    {
        if (!await context.Boards.AnyAsync())
        {
            context.Boards.Add(new Board("Effi Tech Challenge Kanban Board")
            {
                Id = Guid.Parse("6337ba0a-9725-4334-a606-ea62c8cdbc7c")
            });

            await context.SaveChangesAsync();
        }
    }
}
