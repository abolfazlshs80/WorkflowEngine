using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities;

public class ExecutionLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowExecutionId { get; set; }
    public Guid? NodeExecutionId { get; set; }
    public LogLevel Level { get; set; } = LogLevel.Info;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public WorkflowExecution? WorkflowExecution { get; set; }
}
