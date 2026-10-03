using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities;

public class WorkflowExecution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowId { get; set; }
    public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? InputJson { get; set; }
    public string? OutputJson { get; set; }

    public Workflow? Workflow { get; set; }
    public ICollection<NodeExecution> NodeExecutions { get; set; } = new List<NodeExecution>();
    public ICollection<ExecutionLog> Logs { get; set; } = new List<ExecutionLog>();
}
