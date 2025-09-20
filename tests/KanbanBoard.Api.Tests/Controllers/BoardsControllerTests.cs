using FluentAssertions;
using KanbanBoard.Api.Controllers;
using KanbanBoard.Api.Models.Boards;
using KanbanBoard.Application.Exceptions;
using KanbanBoard.Application.Interfaces;
using KanbanBoard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KanbanBoard.Api.Tests.Controllers;

public class BoardsControllerTests
{
    private readonly Mock<IBoardService> _mockService = new();

    [Fact]
    public async Task GetAll_ShouldReturnBoards()
    {
        var boards = new List<Board>
        {
            new("Board 1"),
            new("Board 2")
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        var controller = new BoardsController(_mockService.Object);

        ActionResult<IEnumerable<BoardResponse>> result = await controller.GetAll(CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = (okResult.Value as IEnumerable<BoardResponse>)?.ToList();
        response!.Count.Should().Be(2);
        response.Select(r => r.Name).Should().Contain(["Board 1", "Board 2"]);
    }

    [Fact]
    public async Task GetById_ShouldReturnBoard_WhenExists()
    {
        var board = new Board("My Board");
        _mockService.Setup(s => s.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        var controller = new BoardsController(_mockService.Object);

        ActionResult<BoardResponse> result = await controller.GetById(board.Id, CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as BoardResponse;
        response!.Id.Should().Be(board.Id);
        response.Name.Should().Be("My Board");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenDoesNotExist()
    {
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BoardNotFoundException(Guid.NewGuid()));

        var controller = new BoardsController(_mockService.Object);

        ActionResult<BoardResponse> result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedBoard()
    {
        var request = new BoardRequest("New Board");
        _mockService.Setup(s => s.CreateAsync(It.IsAny<Board>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board b, string? key, CancellationToken ct) => b);

        var controller = new BoardsController(_mockService.Object);

        ActionResult<BoardResponse> result = await controller.Create(request, "idempotency-key", CancellationToken.None);

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        var response = createdResult!.Value as BoardResponse;
        response!.Name.Should().Be("New Board");
    }

    [Fact]
    public async Task Update_ShouldReturnUpdatedBoard_WhenExists()
    {
        var board = new Board("Old Name");
        var request = new BoardRequest("Updated Name");
        _mockService.Setup(s => s.UpdateAsync(board.Id, It.IsAny<Board>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Board("Updated Name") { Id = board.Id });

        var controller = new BoardsController(_mockService.Object);

        ActionResult<BoardResponse> result = await controller.Update(board.Id, request, CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as BoardResponse;
        response!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        var request = new BoardRequest("Updated Name");
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Board>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BoardNotFoundException(Guid.NewGuid()));

        var controller = new BoardsController(_mockService.Object);

        ActionResult<BoardResponse> result = await controller.Update(Guid.NewGuid(), request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldRemoveBoard_WhenExists()
    {
        var boardId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(boardId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var controller = new BoardsController(_mockService.Object);

        IActionResult result = await controller.Delete(boardId, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_EvenIfNotFound()
    {
        _mockService.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BoardNotFoundException(Guid.NewGuid()));

        var controller = new BoardsController(_mockService.Object);

        IActionResult result = await controller.Delete(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }
}
