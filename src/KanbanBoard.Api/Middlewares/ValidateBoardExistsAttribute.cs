using KanbanBoard.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KanbanBoard.Api.Middlewares;

public class ValidateBoardExistsAttribute(KanbanDbContext db) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("boardId", out var boardIdObj) &&
            boardIdObj is Guid boardId)
        {
            var exists = db.Boards.Any(b => b.Id == boardId);
            if (!exists)
            {
                context.Result = new NotFoundObjectResult(new { message = $"Board {boardId} not found" });
            }
        }

        base.OnActionExecuting(context);
    }
}
