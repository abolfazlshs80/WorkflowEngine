namespace WorkflowEngine.Domain.Enums;

public enum WorkflowStatus
{
    Draft = 0,
    Active = 1,
    Archived = 2
}

public enum ExecutionStatus
{
    Pending = 0,
    Running = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}

public enum NodeExecutionStatus
{
    Pending = 0,
    Running = 1,
    Completed = 2,
    Failed = 3,
    Skipped = 4
}

public enum NodeCategory
{
    Trigger = 0,
    Action = 1,
    Condition = 2,
    End = 3
}

public enum LogLevel
{
    Debug = 0,
    Info = 1,
    Warning = 2,
    Error = 3
}
