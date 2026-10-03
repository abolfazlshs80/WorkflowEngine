using System.Text.Json;
using WorkflowEngine.Domain.Abstractions;
using ExecutionContext = WorkflowEngine.Domain.Abstractions.ExecutionContext;

using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Nodes;

public class ManualTriggerNode : INode
{
    public string Type => "trigger.manual";
    public NodeCategory Category => NodeCategory.Trigger;

    public Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct)
        => Task.FromResult(new NodeResult { Output = new { triggeredAt = DateTime.UtcNow, source = "manual" } });
}

public class ApiTriggerNode : INode
{
    public string Type => "trigger.api";
    public NodeCategory Category => NodeCategory.Trigger;

    public Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct)
        => Task.FromResult(new NodeResult { Output = new { triggeredAt = DateTime.UtcNow, source = "api" } });
}

public class SchedulerTriggerNode : INode
{
    public string Type => "trigger.scheduler";
    public NodeCategory Category => NodeCategory.Trigger;

    public Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<JsonElement>(node.ConfigurationJson ?? "{}");
        var cron = config.TryGetProperty("cron", out var c) ? c.GetString() : null;
        return Task.FromResult(new NodeResult { Output = new { triggeredAt = DateTime.UtcNow, source = "scheduler", cron } });
    }
}


