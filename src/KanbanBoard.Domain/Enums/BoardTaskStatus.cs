using System.Runtime.Serialization;

namespace KanbanBoard.Domain.Enums;

public enum BoardTaskStatus
{
    [EnumMember(Value = "toDo")]
    ToDo = 0,

    [EnumMember(Value = "inProgress")]
    InProgress = 1,

    [EnumMember(Value = "done")]
    Done = 2
}
