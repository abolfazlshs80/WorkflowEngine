using System.Text.Json;
using WorkflowEngine.Domain.Abstractions;
using ExecutionContext = WorkflowEngine.Domain.Abstractions.ExecutionContext;

using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Nodes;

public class ConditionNode : INode
{
    public string Type => "condition";
    public NodeCategory Category => NodeCategory.Condition;

    public Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<JsonElement>(node.ConfigurationJson ?? "{}");
        var variable = config.TryGetProperty("variable", out var v) ? v.GetString() : null;
        var op = config.TryGetProperty("operator", out var o) ? o.GetString() ?? "==" : "==";
        var expected = config.TryGetProperty("value", out var val) ? val.GetString() : null;

        var actual = variable is not null && context.Variables.TryGetValue(variable, out var av)
            ? av?.ToString()
            : null;

        var ok = op switch
        {
            "==" => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
            "!=" => !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
            ">" => double.TryParse(actual, out var a1) && double.TryParse(expected, out var e1) && a1 > e1,
            "<" => double.TryParse(actual, out var a2) && double.TryParse(expected, out var e2) && a2 < e2,
            "contains" => actual?.Contains(expected ?? "", StringComparison.OrdinalIgnoreCase) ?? false,
            _ => false
        };

        return Task.FromResult(new NodeResult
        {
            Output = new { variable, actual, expected, result = ok },
            NextPort = ok ? "true" : "false"
        });
    }
}

public class EndNode : INode
{
    public string Type => "end";
    public NodeCategory Category => NodeCategory.End;

    public Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct)
        => Task.FromResult(new NodeResult { Output = new { finishedAt = DateTime.UtcNow } });
}


