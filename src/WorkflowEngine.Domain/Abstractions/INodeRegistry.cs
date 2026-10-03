namespace WorkflowEngine.Domain.Abstractions;

/// <summary>
/// Open/Closed: new nodes are added by registering them here — the Executor never changes.
/// </summary>
public interface INodeRegistry
{
    void Register(INode node);
    INode? Resolve(string nodeType);
    IReadOnlyCollection<INode> GetAll();
}
