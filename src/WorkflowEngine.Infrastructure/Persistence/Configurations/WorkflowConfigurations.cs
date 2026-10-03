using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Infrastructure.Persistence.Configurations;

public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> b)
    {
        b.ToTable("Workflows");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.HasMany(x => x.Nodes).WithOne(n => n.Workflow).HasForeignKey(n => n.WorkflowId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Edges).WithOne(e => e.Workflow).HasForeignKey(e => e.WorkflowId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Executions).WithOne(e => e.Workflow).HasForeignKey(e => e.WorkflowId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkflowNodeConfiguration : IEntityTypeConfiguration<WorkflowNode>
{
    public void Configure(EntityTypeBuilder<WorkflowNode> b)
    {
        b.ToTable("WorkflowNodes");
        b.HasKey(x => x.Id);
        b.Property(x => x.NodeKey).HasMaxLength(64).IsRequired();
        b.Property(x => x.NodeType).HasMaxLength(100).IsRequired();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.HasIndex(x => new { x.WorkflowId, x.NodeKey }).IsUnique();
    }
}

public class WorkflowEdgeConfiguration : IEntityTypeConfiguration<WorkflowEdge>
{
    public void Configure(EntityTypeBuilder<WorkflowEdge> b)
    {
        b.ToTable("WorkflowEdges");
        b.HasKey(x => x.Id);
        b.Property(x => x.SourcePort).HasMaxLength(20);
        b.Property(x => x.Label).HasMaxLength(100);
        b.HasIndex(x => x.SourceNodeId);
        b.HasIndex(x => x.TargetNodeId);
    }
}

public class WorkflowExecutionConfiguration : IEntityTypeConfiguration<WorkflowExecution>
{
    public void Configure(EntityTypeBuilder<WorkflowExecution> b)
    {
        b.ToTable("WorkflowExecutions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.HasIndex(x => x.WorkflowId);
        b.HasMany(x => x.NodeExecutions).WithOne(n => n.WorkflowExecution).HasForeignKey(n => n.WorkflowExecutionId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Logs).WithOne(l => l.WorkflowExecution).HasForeignKey(l => l.WorkflowExecutionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class NodeExecutionConfiguration : IEntityTypeConfiguration<NodeExecution>
{
    public void Configure(EntityTypeBuilder<NodeExecution> b)
    {
        b.ToTable("NodeExecutions");
        b.HasKey(x => x.Id);
        b.Property(x => x.NodeType).HasMaxLength(100).IsRequired();
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.HasIndex(x => x.WorkflowExecutionId);
    }
}

public class ExecutionLogConfiguration : IEntityTypeConfiguration<ExecutionLog>
{
    public void Configure(EntityTypeBuilder<ExecutionLog> b)
    {
        b.ToTable("ExecutionLogs");
        b.HasKey(x => x.Id);
        b.Property(x => x.Level).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Message).HasMaxLength(4000).IsRequired();
        b.HasIndex(x => x.WorkflowExecutionId);
    }
}
