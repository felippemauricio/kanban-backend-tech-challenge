using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KanbanBoard.Api.Models.Boards;

[method: JsonConstructor]
public class BoardRequest(string name)
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(5, ErrorMessage = "Name must be at least 5 characters")]
    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string Name { get; } = name;
}
