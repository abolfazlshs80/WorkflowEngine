using System.Text;
using System.Text.Json;
using WorkflowEngine.Domain.Abstractions;
using ExecutionContext = WorkflowEngine.Domain.Abstractions.ExecutionContext;

using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Nodes;

public class LogActionNode : INode
{
    public string Type => "action.log";
    public NodeCategory Category => NodeCategory.Action;

    public Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<JsonElement>(node.ConfigurationJson ?? "{}");
        var message = config.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";

        // Simple token replacement: {variableName}
        foreach (var kv in context.Variables)
            message = message.Replace("{" + kv.Key + "}", kv.Value?.ToString());

        return Task.FromResult(new NodeResult { Output = new { message } });
    }
}


public class HttpRequestNode : INode
{
    private readonly IHttpClientFactory _httpFactory;

    public HttpRequestNode(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

    public string Type => "action.http";
    public NodeCategory Category => NodeCategory.Action;

    public async Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<JsonElement>(node.ConfigurationJson ?? "{}");
        var url = config.TryGetProperty("url", out var u) ? u.GetString() : null;
        var method = config.TryGetProperty("method", out var m) ? m.GetString() ?? "GET" : "GET";
        var body = config.TryGetProperty("body", out var b) ? b.GetString() : null;

        if (string.IsNullOrWhiteSpace(url))
            return new NodeResult { Success = false, ErrorMessage = "HTTP node requires a 'url'." };

        var client = _httpFactory.CreateClient("workflow");
        var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (!string.IsNullOrEmpty(body))
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);
        return new NodeResult
        {
            Success = response.IsSuccessStatusCode,
            Output = new { statusCode = (int)response.StatusCode, body = responseBody },
            ErrorMessage = response.IsSuccessStatusCode ? null : $"HTTP {(int)response.StatusCode}"
        };
    }
}

public class DelayNode : INode
{
    public string Type => "action.delay";
    public NodeCategory Category => NodeCategory.Action;

    public async Task<NodeResult> ExecuteAsync(WorkflowNode node, ExecutionContext context, CancellationToken ct)
    {
        var config = JsonSerializer.Deserialize<JsonElement>(node.ConfigurationJson ?? "{}");
        var ms = config.TryGetProperty("milliseconds", out var m) ? m.GetInt32() : 1000;
        await Task.Delay(ms, ct);
        return new NodeResult { Output = new { delayedMs = ms } };
    }
}


