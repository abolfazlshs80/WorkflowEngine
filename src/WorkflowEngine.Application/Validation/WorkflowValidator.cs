using WorkflowEngine.Domain.Abstractions;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Validation;

public class WorkflowValidator : IWorkflowValidator
{
    private readonly INodeRegistry _registry;

    public WorkflowValidator(INodeRegistry registry) => _registry = registry;

    public ValidationResult Validate(Workflow workflow)
    {
        var result = new ValidationResult();

        if (workflow.Nodes.Count == 0)
            result.Errors.Add("Workflow has no nodes.");

        // Exactly one start node (no incoming edges)
        var startNodes = workflow.Nodes
            .Where(n => !workflow.Edges.Any(e => e.TargetNodeId == n.Id))
            .ToList();
        if (startNodes.Count != 1)
            result.Errors.Add($"Workflow must have exactly one start node, found {startNodes.Count}.");

        // Start node should be a trigger
        if (startNodes.Count == 1 && _registry.Resolve(startNodes[0].NodeType)?.Category != NodeCategory.Trigger)
            result.Errors.Add("Start node must be a trigger node.");

        // At least one End node
        var endNodes = workflow.Nodes
            .Where(n => _registry.Resolve(n.NodeType)?.Category == NodeCategory.End)
            .ToList();
        if (endNodes.Count == 0)
            result.Errors.Add("Workflow must contain at least one End node.");

        // All node types must exist in registry
        foreach (var node in workflow.Nodes)
        {
            if (_registry.Resolve(node.NodeType) is null)
                result.Errors.Add($"Unknown node type '{node.NodeType}' on node '{node.Name}'.");
        }

        // Edges must reference existing nodes
        var nodeIds = workflow.Nodes.Select(n => n.Id).ToHashSet();
        foreach (var edge in workflow.Edges)
        {
            if (!nodeIds.Contains(edge.SourceNodeId) || !nodeIds.Contains(edge.TargetNodeId))
                result.Errors.Add("An edge references a missing node.");
        }

        // No disconnected nodes (every non-start node must be reachable via incoming edges)
        foreach (var node in workflow.Nodes.Where(n => !startNodes.Contains(n)))
        {
            if (!workflow.Edges.Any(e => e.TargetNodeId == node.Id))
                result.Errors.Add($"Node '{node.Name}' is disconnected (no incoming edge).");
        }

        // Cycle detection (simple DFS — workflows should be acyclic)
        if (HasCycle(workflow))
            result.Errors.Add("Workflow graph contains a cycle.");

        return result;
    }

    private static bool HasCycle(Workflow workflow)
    {
        var visited = new HashSet<Guid>();
        var stack = new HashSet<Guid>();

        bool Visit(Guid id)
        {
            if (stack.Contains(id)) return true;
            if (visited.Contains(id)) return false;
            visited.Add(id);
            stack.Add(id);
            foreach (var e in workflow.Edges.Where(e => e.SourceNodeId == id))
                if (Visit(e.TargetNodeId)) return true;
            stack.Remove(id);
            return false;
        }

        return workflow.Nodes.Any(n => Visit(n.Id));
    }
}
