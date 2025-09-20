using FluentAssertions;
using KanbanBoard.Api.Controllers;
using KanbanBoard.Api.Models.Tasks;
using KanbanBoard.Application.Exceptions;
using KanbanBoard.Application.Interfaces;
using KanbanBoard.Domain.Entities;
using KanbanBoard.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KanbanBoard.Api.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<IBoardTaskService> _mockService = new();

        [Fact]
        public async Task GetAll_ShouldReturnTasks()
        {
            var boardId = Guid.NewGuid();
            var tasks = new List<BoardTask>
            {
                new(boardId, "Task 1"),
                new(boardId, "Task 2")
            };

            _mockService.Setup(s => s.GetAllAsync(boardId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(tasks);

            var controller = new TasksController(_mockService.Object);
            ActionResult<IEnumerable<TaskResponse>> result = await controller.GetAll(boardId, CancellationToken.None);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult!.Value as IEnumerable<TaskResponse>;
            response!.Select(r => r.Title).Should().Contain(["Task 1", "Task 2"]);
        }

        [Fact]
        public async Task GetById_ShouldReturnTask_WhenExists()
        {
            var boardId = Guid.NewGuid();
            var task = new BoardTask(boardId, "Task 1");

            _mockService.Setup(s => s.GetByIdAsync(boardId, task.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(task);

            var controller = new TasksController(_mockService.Object);
            ActionResult<TaskResponse> result = await controller.GetById(boardId, task.Id, CancellationToken.None);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult!.Value as TaskResponse;
            response!.Title.Should().Be("Task 1");
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            var boardId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            _mockService.Setup(s => s.GetByIdAsync(boardId, taskId, It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new BoardTaskNotFoundException(taskId));

            var controller = new TasksController(_mockService.Object);
            ActionResult<TaskResponse> result = await controller.GetById(boardId, taskId, CancellationToken.None);

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedTask()
        {
            var boardId = Guid.NewGuid();
            var request = new TaskRequest("New Task", "New Desc");

            _mockService
                .Setup(s => s.CreateAsync(boardId, It.IsAny<BoardTask>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid bId, BoardTask boardTask, string? key, CancellationToken ct) => boardTask);

            var controller = new TasksController(_mockService.Object);
            ActionResult<TaskResponse> result = await controller.Create(boardId, request, "idempotency-key", CancellationToken.None);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            var response = createdResult!.Value as TaskResponse;
            response!.Title.Should().Be("New Task");
        }

        [Fact]
        public async Task Update_ShouldReturnUpdatedTask()
        {
            var boardId = Guid.NewGuid();
            var task = new BoardTask(boardId, "Old Task");
            var request = new TaskRequest("Updated Task", "Updated Desc");

            _mockService.Setup(s => s.UpdateAsync(boardId, task.Id, It.IsAny<BoardTask>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new BoardTask(boardId, "Updated Task") { Id = task.Id });

            var controller = new TasksController(_mockService.Object);
            ActionResult<TaskResponse> result = await controller.Update(boardId, task.Id, request, CancellationToken.None);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult!.Value as TaskResponse;
            response!.Title.Should().Be("Updated Task");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenTaskExists()
        {
            var boardId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            _mockService.Setup(s => s.DeleteAsync(boardId, taskId, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var controller = new TasksController(_mockService.Object);
            IActionResult result = await controller.Delete(boardId, taskId, CancellationToken.None);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenTaskDoesNotExist()
        {
            var boardId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            _mockService.Setup(s => s.DeleteAsync(boardId, taskId, It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new BoardNotFoundException(taskId));

            var controller = new TasksController(_mockService.Object);
            IActionResult result = await controller.Delete(boardId, taskId, CancellationToken.None);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Move_ShouldReturnMovedTask()
        {
            var boardId = Guid.NewGuid();
            var task = new BoardTask(boardId, "Task 1");
            var request = new MoveTaskRequest(BoardTaskStatus.InProgress, null, null);

            _mockService.Setup(s => s.MoveAsync(boardId, task.Id, request.Status, null, null, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new BoardTask(boardId, "Task 1") { Id = task.Id, Status = request.Status });

            var controller = new TasksController(_mockService.Object);
            IActionResult result = await controller.Move(boardId, task.Id, request, CancellationToken.None);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult!.Value as TaskResponse;
            response!.Status.Should().Be(Domain.Enums.BoardTaskStatus.InProgress);
        }
    }
}
