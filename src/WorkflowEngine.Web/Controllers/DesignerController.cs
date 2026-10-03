using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Application.Abstractions;
using WorkflowEngine.Web.Models;

namespace WorkflowEngine.Web.Controllers;

public class DesignerController : Controller
{
    private readonly IWorkflowService _service;

    public DesignerController(IWorkflowService service) => _service = service;

    public async Task<IActionResult> Index(Guid id, CancellationToken ct)
    {
        var workflow = await _service.GetDetailsAsync(id, ct);
        return workflow is null ? NotFound() : View(workflow);
    }
}
