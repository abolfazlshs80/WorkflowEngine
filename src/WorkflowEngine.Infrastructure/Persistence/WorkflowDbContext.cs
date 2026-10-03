using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Infrastructure.Persistence;

public class WorkflowDbContext : DbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options) { }

    public DbSet<Workflow> Workflows => Set<Workflow>();
    public DbSet<WorkflowNode> WorkflowNodes => Set<WorkflowNode>();
    public DbSet<WorkflowEdge> WorkflowEdges => Set<WorkflowEdge>();
    public DbSet<WorkflowExecution> WorkflowExecutions => Set<WorkflowExecution>();
    public DbSet<NodeExecution> NodeExecutions => Set<NodeExecution>();
    public DbSet<ExecutionLog> ExecutionLogs => Set<ExecutionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<WorkflowDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        services.AddScoped<WorkflowEngine.Application.Abstractions.IWorkflowStore, EfWorkflowStore>();
        services.AddScoped<WorkflowEngine.Application.Abstractions.IWorkflowService, EfWorkflowService>();
        return services;
    }
}
