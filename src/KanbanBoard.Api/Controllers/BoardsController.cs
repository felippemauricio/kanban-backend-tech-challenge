using KanbanBoard.Api.Mappers;
using KanbanBoard.Api.Models.Boards;
using KanbanBoard.Application.Exceptions;
using KanbanBoard.Application.Interfaces;
using KanbanBoard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace KanbanBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController(IBoardService boardService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoardResponse>>> GetAll(CancellationToken cancellationToken)
    {
        IEnumerable<Board> boardList = await boardService.GetAllAsync(cancellationToken);
        IEnumerable<BoardResponse> boardResponseList = boardList.Select(board => board.ToResponse());
        return Ok(boardResponseList);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BoardResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Board board = await boardService.GetByIdAsync(id, cancellationToken);
            BoardResponse boardResponse = board.ToResponse();
            return Ok(boardResponse);
        }
        catch (BoardNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// Creates a new board.
    /// This operation is idempotent if the client provides an Idempotency-Key header:
    /// sending the same key multiple times will return the same board without creating duplicates.
    [HttpPost]
    public async Task<ActionResult<BoardResponse>> Create(
        [FromBody] BoardRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        Board board = await boardService.CreateAsync(request.ToDomain(), idempotencyKey, cancellationToken);
        BoardResponse boardResponse = board.ToResponse();
        return CreatedAtAction(nameof(GetById), new { id = boardResponse.Id }, boardResponse);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BoardResponse>> Update(
        Guid id,
        [FromBody] BoardRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            Board board = await boardService.UpdateAsync(id, request.ToDomain(), cancellationToken);
            BoardResponse boardResponse = board.ToResponse();
            return Ok(boardResponse);
        }
        catch (BoardNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// Deletes a board by its ID.
    /// This operation is idempotent: if the board does not exist, it still returns 204 NoContent.
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await boardService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (BoardNotFoundException)
        {
            return NoContent();
        }
    }
}

