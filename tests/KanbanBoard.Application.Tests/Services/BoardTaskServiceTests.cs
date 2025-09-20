using FluentAssertions;
using KanbanBoard.Application.Exceptions;
using KanbanBoard.Application.Services;
using KanbanBoard.Domain.Entities;
using KanbanBoard.Domain.Enums;
using KanbanBoard.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Application.Tests.Services;

public class BoardTaskServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoTasksExist()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);

        IEnumerable<BoardTask> result = await service.GetAllAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnTasks_WhenTheyExist()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);

        var boardId = Guid.NewGuid();

        var task1 = new BoardTask(boardId, "Task 1");
        var task2 = new BoardTask(boardId, "Task 2");
        await service.CreateAsync(boardId, task1, null, CancellationToken.None);
        await service.CreateAsync(boardId, task2, null, CancellationToken.None);

        IEnumerable<BoardTask> result = await service.GetAllAsync(boardId, CancellationToken.None);

        var list = result.ToList();
        list.Should().HaveCount(2);
        list.Select(t => t.Title).Should().Contain(["Task 1", "Task 2"]);
    }

    [Fact]
    public async Task GetAllByStatusAsync_ShouldReturnEmpty_WhenNoTasksExistForStatus()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);

        var boardId = Guid.NewGuid();
        const BoardTaskStatus status = BoardTaskStatus.ToDo;

        IEnumerable<BoardTask> result = await service.GetAllByStatusAsync(boardId, status, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllByStatusAsync_ShouldReturnTasks_WhenTasksExistForStatus()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);

        var boardId = Guid.NewGuid();

        var task1 = new BoardTask(boardId, "Task 1");
        var task2 = new BoardTask(boardId, "Task 2");
        await service.CreateAsync(boardId, task1, null, CancellationToken.None);
        await service.CreateAsync(boardId, task2, null, CancellationToken.None);

        IEnumerable<BoardTask> result = await service.GetAllByStatusAsync(boardId, BoardTaskStatus.ToDo, CancellationToken.None);

        var list = result.ToList();
        list.Should().HaveCount(2);
        list.Select(t => t.Title).Should().Contain(["Task 1", "Task 2"]);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewTask_WithLexoRank()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);
        var boardId = Guid.NewGuid();
        var task = new BoardTask(boardId, "Task 1");

        BoardTask created = await service.CreateAsync(boardId, task, null, CancellationToken.None);

        created.Id.Should().NotBeEmpty();
        created.LexoRankId.Should().NotBeNullOrWhiteSpace();
        (await db.BoardTasks.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnExisting_WhenIdempotencyKeyMatches()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);
        var boardId = Guid.NewGuid();
        const string key = "unique-task-key";

        BoardTask first = await service.CreateAsync(boardId, new BoardTask(boardId, "Task A"), key, CancellationToken.None);
        BoardTask second = await service.CreateAsync(boardId, new BoardTask(boardId, "Task B"), key, CancellationToken.None);

        second.Id.Should().Be(first.Id);
        second.Title.Should().Be("Task A");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenTaskDoesNotExist()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);

        Func<Task> act = async () => await service.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<BoardTaskNotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenExists()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);

        var task = new BoardTask(Guid.NewGuid(), "Task 1");
        await service.CreateAsync(Guid.NewGuid(), task, null, CancellationToken.None);

        BoardTask result = await service.GetByIdAsync(task.BoardId, task.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldChangeTitleAndDescription()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);
        var boardId = Guid.NewGuid();

        BoardTask task = await service.CreateAsync(boardId, new BoardTask(boardId, "Old title") { Description = "Old desc" }, null, CancellationToken.None);
        var updatedTask = new BoardTask(boardId, "New title") { Description = "New desc" };

        BoardTask result = await service.UpdateAsync(boardId, task.Id, updatedTask, CancellationToken.None);

        result.Title.Should().Be("New title");
        result.Description.Should().Be("New desc");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTask()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);
        var boardId = Guid.NewGuid();

        BoardTask task = await service.CreateAsync(boardId, new BoardTask(boardId, "Task to delete"), null, CancellationToken.None);

        await service.DeleteAsync(boardId, task.Id, CancellationToken.None);

        (await db.BoardTasks.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task MoveAsync_ShouldUpdateStatusAndRank()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);
        var boardId = Guid.NewGuid();

        BoardTask task = await service.CreateAsync(boardId, new BoardTask(boardId, "Task 1"), null, CancellationToken.None);
        BoardTask secondTask = await service.CreateAsync(boardId, new BoardTask(boardId, "Task 2"), null, CancellationToken.None);
        var oldLexoRank = secondTask.LexoRankId;

        BoardTask moved = await service.MoveAsync(boardId, secondTask.Id, BoardTaskStatus.InProgress, null, null, CancellationToken.None);

        moved.Status.Should().Be(BoardTaskStatus.InProgress);
        moved.LexoRankId.Should().NotBe(oldLexoRank);
    }

    [Fact]
    public async Task GetTasksGroupedByStatusAsync_ShouldReturnAllStatuses()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);
        var boardId = Guid.NewGuid();

        await service.CreateAsync(boardId, new BoardTask(boardId, "Task 1"), null, CancellationToken.None);
        BoardTask secondTask = await service.CreateAsync(boardId, new BoardTask(boardId, "Task 2"), null, CancellationToken.None);
        await service.MoveAsync(boardId, secondTask.Id, BoardTaskStatus.Done, null, null, CancellationToken.None);

        IDictionary<BoardTaskStatus, IList<BoardTask>> grouped = await service.GetTasksGroupedByStatusAsync(boardId, CancellationToken.None);

        grouped.Keys.Should().Contain(BoardTaskStatus.ToDo);
        grouped.Keys.Should().Contain(BoardTaskStatus.InProgress);
        grouped.Keys.Should().Contain(BoardTaskStatus.Done);

        grouped[BoardTaskStatus.ToDo].Should().HaveCount(1);
        grouped[BoardTaskStatus.Done].Should().HaveCount(1);
        grouped[BoardTaskStatus.InProgress].Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllStatuses_ShouldReturnAllEnumNames()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardTaskService(db);

        IEnumerable<string> result = service.GetAllStatuses(Guid.NewGuid());

        var expectedStatuses = Enum.GetNames<BoardTaskStatus>();
        result.Should().BeEquivalentTo(expectedStatuses);
    }

    private static KanbanDbContext GetDbContext()
    {
        DbContextOptions<KanbanDbContext> options = new DbContextOptionsBuilder<KanbanDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new KanbanDbContext(options);
    }
}
