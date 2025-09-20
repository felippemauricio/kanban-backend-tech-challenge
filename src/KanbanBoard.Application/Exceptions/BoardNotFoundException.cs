namespace KanbanBoard.Application.Exceptions;

public class BoardNotFoundException(Guid guid) : Exception($"Board {guid} not found");
