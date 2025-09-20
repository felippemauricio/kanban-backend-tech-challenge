using FluentAssertions;
using KanbanBoard.Application.Exceptions;
using KanbanBoard.Application.Services;
using KanbanBoard.Domain.Entities;
using KanbanBoard.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.Application.Tests.Services;

public class BoardServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnBoards_WhenTheyExist()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardService(db);

        var board1 = new Board("Board One");
        var board2 = new Board("Board Two");

        await service.CreateAsync(board1, null, CancellationToken.None);
        await service.CreateAsync(board2, null, CancellationToken.None);

        IEnumerable<Board> result = await service.GetAllAsync(CancellationToken.None);

        IEnumerable<Board> enumerable = result as Board[] ?? result.ToArray();
        enumerable.Should().HaveCount(2);
        enumerable.Select(b => b.Name).Should().Contain(["Board One", "Board Two"]);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoBoardsExist()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardService(db);

        IEnumerable<Board> result = await service.GetAllAsync(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewBoard()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardService(db);
        var newBoard = new Board("My Board");

        Board created = await service.CreateAsync(newBoard, null, CancellationToken.None);

        created.Should().NotBeNull();
        created.Name.Should().Be("My Board");

        (await db.Boards.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnExisting_WhenIdempotencyKeyMatches()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardService(db);
        var board = new Board("Original Board");
        const string key = "unique-key";

        await service.CreateAsync(board, key, CancellationToken.None);

        var newBoard = new Board("Another Board");
        Board createdAgain = await service.CreateAsync(newBoard, key, CancellationToken.None);

        createdAgain.Id.Should().Be(board.Id);
        createdAgain.Name.Should().Be("Original Board");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenBoardDoesNotExist()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardService(db);

        Func<Task<Board>> act = async () => await service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<BoardNotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBoard_WhenItExists()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardService(db);

        Board board = await service.CreateAsync(new Board("Existing Board"), null, CancellationToken.None);

        Board found = await service.GetByIdAsync(board.Id, CancellationToken.None);

        found.Should().NotBeNull();
        found.Id.Should().Be(board.Id);
        found.Name.Should().Be("Existing Board");
    }

    [Fact]
    public async Task UpdateAsync_ShouldChangeName()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardService(db);
        Board board = await service.CreateAsync(new Board("Old Name"), null, CancellationToken.None);

        Board updated = await service.UpdateAsync(board.Id, new Board("New Name"), CancellationToken.None);

        updated.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBoard()
    {
        await using KanbanDbContext db = GetDbContext();
        var service = new BoardService(db);
        Board board = await service.CreateAsync(new Board("Board to Delete"), null, CancellationToken.None);

        await service.DeleteAsync(board.Id, CancellationToken.None);

        (await db.Boards.AnyAsync()).Should().BeFalse();
    }

    private static KanbanDbContext GetDbContext()
    {
        DbContextOptions<KanbanDbContext> options = new DbContextOptionsBuilder<KanbanDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new KanbanDbContext(options);
    }
}
