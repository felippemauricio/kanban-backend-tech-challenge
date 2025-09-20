using KanbanBoard.Api.Mappers;
using KanbanBoard.Api.Middlewares;
using KanbanBoard.Api.Models.Tasks;
using KanbanBoard.Application.Interfaces;
using KanbanBoard.Domain.Entities;
using KanbanBoard.Domain.Enums;

namespace KanbanBoard.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[ServiceFilter(typeof(ValidateBoardExistsAttribute))]
[Route("api/boards/{boardId:guid}/[controller]")]
public class ColumnsController(IBoardTaskService taskService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<string>> GetTaskStatuses(Guid boardId)
    {
        IEnumerable<string> statuses = taskService.GetAllStatuses(boardId);
        return Ok(statuses);
    }

    [HttpGet("{status}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasksByColumn(
        Guid boardId,
        BoardTaskStatus status,
        CancellationToken cancellationToken)
    {
        IEnumerable<BoardTask> taskList = await taskService.GetAllByStatusAsync(boardId, status, cancellationToken);
        IEnumerable<TaskResponse> taskResponseList = taskList.Select(boardTask => boardTask.ToResponse());
        return Ok(taskResponseList);
    }

    [HttpGet("tasks")]
    public async Task<ActionResult<IDictionary<BoardTaskStatus, IEnumerable<TaskResponse>>>> GetTasksGroupedByColumn(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        IDictionary<BoardTaskStatus, IList<BoardTask>> groupedTasks =
            await taskService.GetTasksGroupedByStatusAsync(boardId, cancellationToken);

        var groupedTaskResponses = groupedTasks.ToDictionary(
            kvp => kvp.Key, IList<TaskResponse> (kvp) => kvp.Value
                .Select(boardTask => boardTask.ToResponse())
                .ToList()
        );

        return Ok(groupedTaskResponses);
    }
}
