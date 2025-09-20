using KanbanBoard.Api.Mappers;
using KanbanBoard.Api.Middlewares;
using KanbanBoard.Api.Models.Tasks;
using KanbanBoard.Application.Exceptions;
using KanbanBoard.Application.Interfaces;
using KanbanBoard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace KanbanBoard.Api.Controllers;

[ApiController]
[ServiceFilter(typeof(ValidateBoardExistsAttribute))]
[Route("api/boards/{boardId:guid}/[controller]")]
public class TasksController(IBoardTaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        IEnumerable<BoardTask> taskList = await taskService.GetAllAsync(boardId, cancellationToken);
        IEnumerable<TaskResponse> taskResponseList = taskList.Select(boardTask => boardTask.ToResponse());
        return Ok(taskResponseList);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid boardId, Guid id, CancellationToken cancellationToken)
    {
        try
        {
            BoardTask boardTask = await taskService.GetByIdAsync(boardId, id, cancellationToken);
            TaskResponse taskResponse = boardTask.ToResponse();
            return Ok(taskResponse);
        }
        catch (BoardTaskNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// Creates a new task.
    /// This operation is idempotent if the client provides an Idempotency-Key header:
    /// sending the same key multiple times will return the same task without creating duplicates.
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(
        Guid boardId,
        [FromBody] TaskRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        BoardTask boardTask = await taskService.CreateAsync(
            boardId,
            request.ToDomain(boardId),
            idempotencyKey,
            cancellationToken);
        TaskResponse taskResponse = boardTask.ToResponse();
        return CreatedAtAction(nameof(GetById), new { boardId, id = taskResponse.Id }, taskResponse);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(
        Guid boardId,
        Guid id,
        [FromBody] TaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            BoardTask boardTask = await taskService.UpdateAsync(
                boardId,
                id,
                request.ToDomain(boardId),
                cancellationToken);
            TaskResponse taskResponse = boardTask.ToResponse();
            return Ok(taskResponse);
        }
        catch (BoardNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// Deletes a task by its ID.
    /// This operation is idempotent: if the task does not exist, it still returns 204 NoContent.
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid boardId, Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await taskService.DeleteAsync(boardId, id, cancellationToken);
            return NoContent();
        }
        catch (BoardNotFoundException)
        {
            return NoContent();
        }
    }

    [HttpPatch("{id:guid}/move")]
    public async Task<IActionResult> Move(
        Guid boardId,
        Guid id,
        [FromBody] MoveTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            BoardTask boardTask = await taskService.MoveAsync(
                boardId,
                id,
                request.Status,
                request.PreviousTaskId,
                request.NextTaskId,
                cancellationToken);
            TaskResponse taskResponse = boardTask.ToResponse();
            return Ok(taskResponse);
        }
        catch (BoardNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
