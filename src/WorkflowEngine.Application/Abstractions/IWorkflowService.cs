using WorkflowEngine.Domain.Entities;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Application.Abstractions;

public interface IWorkflowService
{
    Task<List<Workflow>> GetAllAsync(CancellationToken ct = default);
    Task<Workflow?> GetDetailsAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(string name, string? description, CancellationToken ct = default);
    Task UpdateAsync(Guid id, string name, string? description, WorkflowStatus status, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Dictionary<string, int>> GetDashboardStatsAsync(CancellationToken ct = default);
    Task<List<WorkflowExecution>> GetExecutionsAsync(int take = 100, CancellationToken ct = default);
    Task<WorkflowExecution?> GetExecutionDetailsAsync(Guid id, CancellationToken ct = default);
}
