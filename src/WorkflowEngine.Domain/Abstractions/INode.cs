using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Domain.Abstractions;

/// <summary>
/// A node executes, produces an output and tells the engine which port(s) to follow.
/// </summary>
public interface INode
{
    string Type { get; }              // e.g. "log.action"
    NodeCategory Category { get; }
    Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct);
}

public class NodeResult
{
    public bool Success { get; set; } = true;
    public object? Output { get; set; }
    public string? NextPort { get; set; } // null = default port, "true"/"false" for conditions
    public string? ErrorMessage { get; set; }
}
