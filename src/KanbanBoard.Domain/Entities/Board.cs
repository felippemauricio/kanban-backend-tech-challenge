using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanBoard.Domain.Entities;

[Table("Boards")]
public class Board(string name)
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(50)]
    public string Name { get; private set; } = name;

    [MinLength(36), MaxLength(36)]
    public string? IdempotencyKey { get; private set; }

    [Required]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public void SetIdempotencyKey(string idempotencyKey)
    {
        IdempotencyKey = idempotencyKey;
    }

    public void Update(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}
