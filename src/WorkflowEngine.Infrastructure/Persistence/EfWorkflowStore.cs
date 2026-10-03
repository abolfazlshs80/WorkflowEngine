using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Application.Abstractions;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Infrastructure.Persistence;

public class EfWorkflowStore : IWorkflowStore
{
    private readonly WorkflowDbContext _db;

    public EfWorkflowStore(WorkflowDbContext db) => _db = db;

    public Task<Workflow?> GetWithGraphAsync(Guid id, CancellationToken ct) =>
        _db.Workflows
            .Include(w => w.Nodes)
            .Include(w => w.Edges)
            .FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task AddExecutionAsync(WorkflowExecution execution, CancellationToken ct) =>
        await _db.WorkflowExecutions.AddAsync(execution, ct);

    public async Task AddNodeExecutionAsync(NodeExecution nodeExecution, CancellationToken ct) =>
        await _db.NodeExecutions.AddAsync(nodeExecution, ct);

    public async Task AddLogAsync(ExecutionLog log, CancellationToken ct) =>
        await _db.ExecutionLogs.AddAsync(log, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public async Task ReplaceGraphAsync(Guid workflowId, List<Domain.Entities.WorkflowNode> nodes, List<Domain.Entities.WorkflowEdge> edges, CancellationToken ct)
    {
        var oldNodes = await _db.WorkflowNodes.Where(n => n.WorkflowId == workflowId).ToListAsync(ct);
        var oldEdges = await _db.WorkflowEdges.Where(e => e.WorkflowId == workflowId).ToListAsync(ct);
        _db.WorkflowNodes.RemoveRange(oldNodes);
        _db.WorkflowEdges.RemoveRange(oldEdges);
        await _db.WorkflowNodes.AddRangeAsync(nodes, ct);
        await _db.WorkflowEdges.AddRangeAsync(edges, ct);
        await _db.SaveChangesAsync(ct);
    }
}
