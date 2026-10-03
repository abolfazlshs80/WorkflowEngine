using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Application.Abstractions;
using WorkflowEngine.Application.Execution;
using WorkflowEngine.Domain.Abstractions;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Web.Models;

namespace WorkflowEngine.Web.Controllers.Api;

[ApiController]
[Route("api/designer/{workflowId:guid}")]
public class DesignerApiController : ControllerBase
{
    private readonly IWorkflowStore _store;
    private readonly IWorkflowValidator _validator;
    private readonly IWorkflowExecutor _executor;

    public DesignerApiController(IWorkflowStore store, IWorkflowValidator validator, IWorkflowExecutor executor)
    {
        _store = store;
        _validator = validator;
        _executor = executor;
    }

    [HttpGet("graph")]
    public async Task<ActionResult<DesignerGraphDto>> GetGraph(Guid workflowId, CancellationToken ct)
    {
        var workflow = await _store.GetWithGraphAsync(workflowId, ct);
        if (workflow is null) return NotFound();
        return new DesignerGraphDto
        {
            Nodes = workflow.Nodes.Select(n => new DesignerNodeDto
            {
                Id = n.Id,
                NodeKey = n.NodeKey,
                NodeType = n.NodeType,
                Name = n.Name,
                ConfigurationJson = n.ConfigurationJson,
                X = n.PositionX,
                Y = n.PositionY
            }).ToList(),
            Edges = workflow.Edges.Select(e => new DesignerEdgeDto
            {
                SourceNodeKey = workflow.Nodes.First(n => n.Id == e.SourceNodeId).NodeKey,
                TargetNodeKey = workflow.Nodes.First(n => n.Id == e.TargetNodeId).NodeKey,
                SourcePort = e.SourcePort
            }).ToList()
        };
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(Guid workflowId, [FromBody] DesignerGraphDto dto, CancellationToken ct)
    {
        var workflow = await _store.GetWithGraphAsync(workflowId, ct);
        if (workflow is null) return NotFound();

        var nodes = dto.Nodes.Select(n => new WorkflowNode
        {
            Id = n.Id ?? Guid.NewGuid(),
            WorkflowId = workflowId,
            NodeKey = n.NodeKey,
            NodeType = n.NodeType,
            Name = n.Name,
            ConfigurationJson = n.ConfigurationJson,
            PositionX = n.X,
            PositionY = n.Y
        }).ToList();

        var keyToId = nodes.ToDictionary(n => n.NodeKey, n => n.Id);
        var edges = new List<WorkflowEdge>();
        foreach (var e in dto.Edges)
        {
            if (!keyToId.TryGetValue(e.SourceNodeKey, out var src) || !keyToId.TryGetValue(e.TargetNodeKey, out var tgt))
                return BadRequest($"Edge references unknown node key: {e.SourceNodeKey} -> {e.TargetNodeKey}");
            edges.Add(new WorkflowEdge
            {
                WorkflowId = workflowId,
                SourceNodeId = src,
                TargetNodeId = tgt,
                SourcePort = e.SourcePort
            });
        }

        await _store.ReplaceGraphAsync(workflowId, nodes, edges, ct);
        return Ok(new { saved = true, nodes = nodes.Count, edges = edges.Count });
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Guid workflowId, CancellationToken ct)
    {
        var workflow = await _store.GetWithGraphAsync(workflowId, ct);
        if (workflow is null) return NotFound();
        var result = _validator.Validate(workflow);
        return Ok(new { valid = result.IsValid, errors = result.Errors });
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute(Guid workflowId, CancellationToken ct)
    {
        var workflow = await _store.GetWithGraphAsync(workflowId, ct);
        if (workflow is null) return NotFound();
        try
        {
            var execution = await _executor.ExecuteAsync(workflow, ct: ct);
            return Ok(new { executionId = execution.Id, status = execution.Status.ToString(), error = execution.ErrorMessage });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
