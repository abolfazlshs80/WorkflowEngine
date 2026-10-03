using System.ComponentModel.DataAnnotations;
using WorkflowEngine.Domain.Enums;

namespace WorkflowEngine.Web.Models;

public class WorkflowFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;
}
