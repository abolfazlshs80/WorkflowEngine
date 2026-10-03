namespace WorkflowEngine.Domain.Entities;

public class WorkflowNode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowId { get; set; }
    public string NodeKey { get; set; } = string.Empty; // Unique within the workflow (designer id)
    public string NodeType { get; set; } = string.Empty; // Key in the Node Registry, e.g. "log.action"
    public string Name { get; set; } = string.Empty;
    public string? ConfigurationJson { get; set; } // Per-node settings (e.g. URL, delay ms)
    public double PositionX { get; set; }
    public double PositionY { get; set; }

    public Workflow? Workflow { get; set; }
}
