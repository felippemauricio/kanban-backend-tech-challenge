namespace KanbanBoard.Application.Exceptions;


public class BoardTaskNotFoundException(Guid guid) : Exception($"Task {guid} not found");
