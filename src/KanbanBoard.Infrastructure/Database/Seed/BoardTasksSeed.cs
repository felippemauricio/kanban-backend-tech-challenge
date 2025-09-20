using KanbanBoard.Domain.Entities;
using KanbanBoard.Domain.Enums;
using LexoAlgorithm;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Infrastructure.Database.Seed;

public static class BoardTasksSeed
{
    private static readonly Guid BoardId = Guid.Parse("6337ba0a-9725-4334-a606-ea62c8cdbc7c");

    public static async Task Initialize(KanbanDbContext context)
    {
        if (!await context.BoardTasks.AnyAsync())
        {
            var ranks = new List<string>();
            var current = LexoRank.Middle();

            for (var i = 0; i < 10; i++)
            {
                ranks.Add(current.ToString());
                current = current.GenNext();
            }

            var tasks = new List<BoardTask>
            {
                new(BoardId, "Setup project repository")
                {
                    Description =
                        "Install .NET SDK, configure local database, and ensure the project runs correctly.",
                },
                new(BoardId, "Implement user authentication")
                {
                    Description = "Build signup/login flow with JWT authentication.",
                },
                new(BoardId, "Design Kanban board UI")
                {
                    Description =
                        "Create the initial board layout with To Do, In Progress, and Done columns.",
                },
                new(BoardId, "Add drag and drop functionality")
                {
                    Description = "Enable moving tasks between columns via drag and drop.",
                },
                new(BoardId, "Implement task creation modal")
                {
                    Description = "Add a modal to create new tasks with title and description.",
                },
                new(BoardId, "Add task editing and deleting")
                {
                    Description = "Allow editing task title/description and deleting tasks.",
                    Status = BoardTaskStatus.InProgress
                },
                new(BoardId, "Setup CI/CD pipeline")
                {
                    Description = "Configure GitHub Actions to run lint, tests, and build.",
                    Status = BoardTaskStatus.InProgress
                },
                new(BoardId, "Add user roles and permissions")
                {
                    Description = "Create user roles (admin, member) with different permissions.",
                    Status = BoardTaskStatus.Done
                },
                new(BoardId, "Implement activity log")
                {
                    Description =
                        "Record activities such as task creation, updates, and deletions.",
                    Status = BoardTaskStatus.Done
                },
                new(BoardId, "Add search and filter for tasks")
                {
                    Description =
                        "Enable searching and filtering tasks by title, status, or assignee.",
                    Status = BoardTaskStatus.Done
                }
            };

            for (var i = 0; i < tasks.Count; i++)
            {
                tasks[i].SetLexoRankId(ranks[i]);
                context.BoardTasks.Add(tasks[i]);
            }

            await context.SaveChangesAsync();
        }
    }
}
