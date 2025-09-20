using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KanbanBoard.Api.Models.Tasks;

[method: JsonConstructor]
public class TaskRequest(string title, string? description)
{
    [Required(ErrorMessage = "Title is required")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; } = title;

    [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; } = description;
}
