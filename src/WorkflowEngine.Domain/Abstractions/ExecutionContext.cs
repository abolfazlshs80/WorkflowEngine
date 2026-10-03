namespace WorkflowEngine.Domain.Abstractions;

/// <summary>
/// Runtime state that flows through the graph. Nodes read from and write to it.
/// </summary>
public class ExecutionContext
{
    public Guid WorkflowExecutionId { get; set; }
    public Dictionary<string, object?> Variables { get; } = new();
    public Dictionary<string, object?> NodeOutputs { get; } = new(); // NodeKey -> output
    public string? LastNodeKey { get; set; }
    public List<string> ExecutionPath { get; } = new();

    public void SetVariable(string key, object? value) => Variables[key] = value;
    public T? GetVariable<T>(string key) => Variables.TryGetValue(key, out var v) ? (T?)v : default;
}
