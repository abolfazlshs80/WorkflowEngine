using WorkflowEngine.Domain.Abstractions;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.Execution;

public interface IWorkflowExecutor
{
    Task<WorkflowExecution> ExecuteAsync(Workflow workflow, string? inputJson = null, CancellationToken ct = default);
}

