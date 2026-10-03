using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Domain.Abstractions;

public interface IGraphParser
{
    GraphDefinition Parse(Workflow workflow);
}

public class GraphDefinition
{
    public List<WorkflowNode> Nodes { get; } = new();
    public List<WorkflowEdge> Edges { get; } = new();
    public ILookup<Guid, WorkflowEdge> Outgoing => Edges.ToLookup(e => e.SourceNodeId);
    public WorkflowNode? GetStartNode() =>
        Nodes.FirstOrDefault(n => !Edges.Any(e => e.TargetNodeId == n.Id));
}
