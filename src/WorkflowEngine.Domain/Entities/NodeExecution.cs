using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities;

public class NodeExecution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowExecutionId { get; set; }
    public Guid WorkflowNodeId { get; set; }
    public string NodeType { get; set; } = string.Empty;
    public NodeExecutionStatus Status { get; set; } = NodeExecutionStatus.Pending;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public int Attempt { get; set; } = 1;
    public string? ErrorMessage { get; set; }
    public string? OutputJson { get; set; }

    public WorkflowExecution? WorkflowExecution { get; set; }
}
