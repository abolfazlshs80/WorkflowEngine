namespace WorkflowEngine.Domain.Entities;

public class WorkflowEdge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowId { get; set; }
    public Guid SourceNodeId { get; set; }
    public Guid TargetNodeId { get; set; }
    public string? SourcePort { get; set; } // For condition nodes: "true" / "false"
    public string? Label { get; set; }

    public Workflow? Workflow { get; set; }
}
