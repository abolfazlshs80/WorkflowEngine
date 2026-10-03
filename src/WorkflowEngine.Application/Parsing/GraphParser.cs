using WorkflowEngine.Domain.Abstractions;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Application.Parsing;

public class GraphParser : IGraphParser
{
    public GraphDefinition Parse(Workflow workflow)
    {
        var graph = new GraphDefinition();
        graph.Nodes.AddRange(workflow.Nodes);
        graph.Edges.AddRange(workflow.Edges);
        return graph;
    }
}
