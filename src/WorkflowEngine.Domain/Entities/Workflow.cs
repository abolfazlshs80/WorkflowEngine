using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Entities;

public class Workflow
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<WorkflowNode> Nodes { get; set; } = new List<WorkflowNode>();
    public ICollection<WorkflowEdge> Edges { get; set; } = new List<WorkflowEdge>();
    public ICollection<WorkflowExecution> Executions { get; set; } = new List<WorkflowExecution>();
}
