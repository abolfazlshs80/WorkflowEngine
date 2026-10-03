using System.Text.Json;
using WorkflowEngine.Application.Abstractions;
using WorkflowEngine.Application.Execution;
using WorkflowEngine.Domain.Abstractions;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

using ExecutionContext = WorkflowEngine.Domain.Abstractions.ExecutionContext;

namespace WorkflowEngine.Application.Execution;

public class WorkflowExecutor : IWorkflowExecutor
{
    private readonly INodeRegistry _registry;
    private readonly IGraphParser _parser;
    private readonly IWorkflowValidator _validator;
    private readonly IWorkflowStore _store;
    private readonly RetryPolicy _retry;

    public WorkflowExecutor(INodeRegistry registry, IGraphParser parser, IWorkflowValidator validator,
        IWorkflowStore store, RetryPolicy retry)
    {
        _registry = registry;
        _parser = parser;
        _validator = validator;
        _store = store;
        _retry = retry;
    }

    public async Task<WorkflowExecution> ExecuteAsync(Workflow workflow, string? inputJson = null, CancellationToken ct = default)
    {
        var validation = _validator.Validate(workflow);
        if (!validation.IsValid)
            throw new InvalidOperationException("Workflow is invalid: " + string.Join("; ", validation.Errors));

        var execution = new WorkflowExecution
        {
            WorkflowId = workflow.Id,
            Status = ExecutionStatus.Running,
            InputJson = inputJson
        };
        await _store.AddExecutionAsync(execution, ct);
        await Log(execution, null, LogLevel.Info, $"Workflow '{workflow.Name}' started.", ct);

        var context = new ExecutionContext { WorkflowExecutionId = execution.Id };
        if (!string.IsNullOrWhiteSpace(inputJson))
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(inputJson);
                if (dict is not null)
                    foreach (var kv in dict) context.SetVariable(kv.Key, kv.Value);
            }
            catch { /* input not a flat dictionary — ignored */ }
        }

        var graph = _parser.Parse(workflow);
        var current = graph.GetStartNode();

        try
        {
            while (current is not null)
            {
                context.ExecutionPath.Add(current.NodeKey);
                context.LastNodeKey = current.NodeKey;

                var nodeExecution = new NodeExecution
                {
                    WorkflowExecutionId = execution.Id,
                    WorkflowNodeId = current.Id,
                    NodeType = current.NodeType,
                    Status = NodeExecutionStatus.Running
                };
                await _store.AddNodeExecutionAsync(nodeExecution, ct);
                await Log(execution, nodeExecution, LogLevel.Info, $"Executing node '{current.Name}' ({current.NodeType}).", ct);

                var nodeImpl = _registry.Resolve(current.NodeType)
                    ?? throw new InvalidOperationException($"Node type '{current.NodeType}' is not registered.");

                try
                {
                    NodeResult result = null!;
                    await _retry.ExecuteAsync(async attempt =>
                    {
                        nodeExecution.Attempt = attempt;
                        result = await nodeImpl.ExecuteAsync(current, context, ct);
                        if (!result.Success)
                            throw new InvalidOperationException(result.ErrorMessage ?? "Node failed.");
                        return result;
                    }, ct);

                    nodeExecution.Status = NodeExecutionStatus.Completed;
                    nodeExecution.FinishedAt = DateTime.UtcNow;
                    nodeExecution.OutputJson = JsonSerializer.Serialize(result.Output);
                    context.NodeOutputs[current.NodeKey] = result.Output;
                    await Log(execution, nodeExecution, LogLevel.Info, $"Node '{current.Name}' completed.", ct);

                    // Determine next node via edges (port-aware for conditions)
                    var edges = graph.Outgoing[current.Id]
                        .Where(e => result.NextPort is null
                            ? string.IsNullOrEmpty(e.SourcePort)
                            : string.Equals(e.SourcePort, result.NextPort, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    await _store.SaveChangesAsync(ct);

                    if (nodeImpl.Category == NodeCategory.End || edges.Count == 0)
                        break;

                    current = graph.Nodes.FirstOrDefault(n => n.Id == edges[0].TargetNodeId);
                }
                catch (Exception ex)
                {
                    nodeExecution.Status = NodeExecutionStatus.Failed;
                    nodeExecution.FinishedAt = DateTime.UtcNow;
                    nodeExecution.ErrorMessage = ex.Message;
                    await Log(execution, nodeExecution, LogLevel.Error, $"Node '{current.Name}' failed after retries: {ex.Message}", ct);
                    execution.Status = ExecutionStatus.Failed;
                    execution.ErrorMessage = ex.Message;
                    break;
                }
            }

            if (execution.Status == ExecutionStatus.Running)
                execution.Status = ExecutionStatus.Completed;
        }
        catch (Exception ex)
        {
            execution.Status = ExecutionStatus.Failed;
            execution.ErrorMessage = ex.Message;
        }

        execution.FinishedAt = DateTime.UtcNow;
        await Log(execution, null, LogLevel.Info, $"Workflow finished with status {execution.Status}.", ct);
        await _store.SaveChangesAsync(ct);
        return execution;
    }

    private async Task Log(WorkflowExecution execution, NodeExecution? nodeExecution, LogLevel level, string message, CancellationToken ct)
    {
        var log = new ExecutionLog
        {
            WorkflowExecutionId = execution.Id,
            NodeExecutionId = nodeExecution?.Id,
            Level = level,
            Message = message
        };
        await _store.AddLogAsync(log, ct);
    }
}
