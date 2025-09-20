using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KanbanBoard.Domain.Enums;

namespace KanbanBoard.Domain.Entities;

[Table("BoardTasks")]
public class BoardTask(Guid boardId, string title)
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Title { get; private set; } = title;

    [MinLength(36)]
    [MaxLength(36)]
    public string? IdempotencyKey { get; private set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    [Required]
    public string? LexoRankId { get; private set; }

    [Required]
    public BoardTaskStatus Status { get; set; } = BoardTaskStatus.ToDo;

    [Required]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    [Required]
    [ForeignKey("Boards")]
    public Guid BoardId { get; private set; } = boardId;
    public Board? Board { get; init; }

    public void SetIdempotencyKey(string idempotencyKey)
    {
        IdempotencyKey = idempotencyKey;
    }

    public void SetLexoRankId(string lexoRankId)
    {
        LexoRankId = lexoRankId;
    }

    public void Update(string title, string? description)
    {
        Title = title;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Move(BoardTaskStatus status, string lexoRankId)
    {
        Status = status;
        LexoRankId = lexoRankId;
        UpdatedAt = DateTime.UtcNow;
    }
}
