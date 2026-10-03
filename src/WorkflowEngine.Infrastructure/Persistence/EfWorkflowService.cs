using Microsoft.EntityFrameworkCore;
using WorkflowEngine.Application.Abstractions;
using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Infrastructure.Persistence;

public class EfWorkflowService : IWorkflowService
{
    private readonly WorkflowDbContext _db;

    public EfWorkflowService(WorkflowDbContext db) => _db = db;

    public Task<List<Workflow>> GetAllAsync(CancellationToken ct = default) =>
        _db.Workflows.OrderByDescending(w => w.CreatedAt).ToListAsync(ct);

    public Task<Workflow?> GetDetailsAsync(Guid id, CancellationToken ct = default) =>
        _db.Workflows
            .Include(w => w.Nodes)
            .Include(w => w.Edges)
            .Include(w => w.Executions.OrderByDescending(e => e.StartedAt).Take(10))
            .FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<Guid> CreateAsync(string name, string? description, CancellationToken ct = default)
    {
        var workflow = new Workflow { Name = name, Description = description };
        _db.Workflows.Add(workflow);
        await _db.SaveChangesAsync(ct);
        return workflow.Id;
    }

    public async Task UpdateAsync(Guid id, string name, string? description, WorkflowStatus status, CancellationToken ct = default)
    {
        var workflow = await _db.Workflows.FindAsync(new object[] { id }, ct)
            ?? throw new KeyNotFoundException($"Workflow {id} not found.");
        workflow.Name = name;
        workflow.Description = description;
        workflow.Status = status;
        workflow.UpdatedAt = DateTime.UtcNow;
        workflow.Version++;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var workflow = await _db.Workflows.FindAsync(new object[] { id }, ct);
        if (workflow is not null)
        {
            _db.Workflows.Remove(workflow);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<Dictionary<string, int>> GetDashboardStatsAsync(CancellationToken ct = default) => new()
    {
        ["Total workflows"] = await _db.Workflows.CountAsync(ct),
        ["Active"] = await _db.Workflows.CountAsync(w => w.Status == WorkflowStatus.Active, ct),
        ["Draft"] = await _db.Workflows.CountAsync(w => w.Status == WorkflowStatus.Draft, ct),
        ["Executions (24h)"] = await _db.WorkflowExecutions.CountAsync(e => e.StartedAt >= DateTime.UtcNow.AddDays(-1), ct),
        ["Failed executions"] = await _db.WorkflowExecutions.CountAsync(e => e.Status == ExecutionStatus.Failed, ct)
    };

    public Task<List<WorkflowExecution>> GetExecutionsAsync(int take = 100, CancellationToken ct = default) =>
        _db.WorkflowExecutions
            .Include(e => e.Workflow)
            .OrderByDescending(e => e.StartedAt)
            .Take(take)
            .ToListAsync(ct);

    public Task<WorkflowExecution?> GetExecutionDetailsAsync(Guid id, CancellationToken ct = default) =>
        _db.WorkflowExecutions
            .Include(e => e.Workflow)
            .Include(e => e.NodeExecutions.OrderBy(n => n.StartedAt))
            .Include(e => e.Logs.OrderBy(l => l.Timestamp))
            .FirstOrDefaultAsync(e => e.Id == id, ct);
}
