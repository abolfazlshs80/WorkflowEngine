using WorkflowEngine.Domain.Abstractions;

namespace WorkflowEngine.Application.Registry;

public class NodeRegistry : INodeRegistry
{
    private readonly Dictionary<string, INode> _nodes = new(StringComparer.OrdinalIgnoreCase);

    public void Register(INode node) => _nodes[node.Type] = node;
    public INode? Resolve(string nodeType) => _nodes.TryGetValue(nodeType, out var n) ? n : null;
    public IReadOnlyCollection<INode> GetAll() => _nodes.Values.ToList();
}
