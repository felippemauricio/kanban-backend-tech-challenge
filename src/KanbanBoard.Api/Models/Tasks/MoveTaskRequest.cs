using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KanbanBoard.Domain.Enums;

namespace KanbanBoard.Api.Models.Tasks;

public class MoveTaskRequest(BoardTaskStatus status, Guid? previousTaskId, Guid? nextTaskId)
{
    [Required(ErrorMessage = "Status is required")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BoardTaskStatus Status { get; } = status;

    public Guid? PreviousTaskId { get; } = previousTaskId;

    public Guid? NextTaskId { get; } = nextTaskId;
}

