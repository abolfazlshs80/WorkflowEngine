using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Abstractions;

/// <summary>Persistence ports implemented by Infrastructure — keeps Application EF-free.</summary>
public interface IWorkflowStore
{
    Task<Workflow?> GetWithGraphAsync(Guid id, CancellationToken ct);
    Task AddExecutionAsync(WorkflowExecution execution, CancellationToken ct);
    Task AddNodeExecutionAsync(NodeExecution nodeExecution, CancellationToken ct);
    Task AddLogAsync(ExecutionLog log, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task ReplaceGraphAsync(Guid workflowId, List<Domain.Entities.WorkflowNode> nodes, List<Domain.Entities.WorkflowEdge> edges, CancellationToken ct);
}
