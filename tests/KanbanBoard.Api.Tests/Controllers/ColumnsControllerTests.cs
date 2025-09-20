using FluentAssertions;
using KanbanBoard.Api.Controllers;
using KanbanBoard.Api.Models.Tasks;
using KanbanBoard.Application.Interfaces;
using KanbanBoard.Domain.Entities;
using KanbanBoard.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KanbanBoard.Api.Tests.Controllers;

public class ColumnsControllerTests
{
    private readonly Mock<IBoardTaskService> _serviceMock = new Mock<IBoardTaskService>();

    [Fact]
    public async Task GetTasksByColumn_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        var boardId = Guid.NewGuid();
        const BoardTaskStatus status = BoardTaskStatus.ToDo;

        _serviceMock
            .Setup(s => s.GetAllByStatusAsync(boardId, status, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BoardTask>());

        var controller = new ColumnsController(_serviceMock.Object);

        ActionResult<IEnumerable<TaskResponse>> result =
            await controller.GetTasksByColumn(boardId, status, CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var tasks = okResult!.Value as IEnumerable<TaskResponse>;
        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTasksByColumn_ShouldReturnTasks_WhenTasksExist()
    {
        var boardId = Guid.NewGuid();
        const BoardTaskStatus status = BoardTaskStatus.ToDo;

        var task1 = new BoardTask(boardId, "Task 1");
        var task2 = new BoardTask(boardId, "Task 2");

        _serviceMock
            .Setup(s => s.GetAllByStatusAsync(boardId, status, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BoardTask> { task1, task2 });

        var controller = new ColumnsController(_serviceMock.Object);

        ActionResult<IEnumerable<TaskResponse>> result =
            await controller.GetTasksByColumn(boardId, status, CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var tasks = okResult!.Value as IEnumerable<TaskResponse>;
        var taskResponses = tasks!.ToList();
        taskResponses.Should().HaveCount(2);
        taskResponses.Select(t => t.Title).Should().Contain(new[] { "Task 1", "Task 2" });
    }

    [Fact]
    public void GetTaskStatuses_ShouldReturnAllStatuses()
    {
        var boardId = Guid.NewGuid();
        var expectedStatuses = Enum.GetNames<BoardTaskStatus>().ToList();

        _serviceMock
            .Setup(s => s.GetAllStatuses(boardId))
            .Returns(expectedStatuses);

        var controller = new ColumnsController(_serviceMock.Object);

        ActionResult<IEnumerable<string>> result = controller.GetTaskStatuses(boardId);

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var statuses = okResult!.Value as IEnumerable<string>;
        statuses!.Should().BeEquivalentTo(expectedStatuses);
    }
}
