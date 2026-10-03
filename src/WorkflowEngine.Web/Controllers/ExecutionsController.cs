using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Application.Abstractions;

namespace WorkflowEngine.Web.Controllers;

public class ExecutionsController : Controller
{
    private readonly IWorkflowService _service;

    public ExecutionsController(IWorkflowService service) => _service = service;

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _service.GetExecutionsAsync(ct: ct));

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var execution = await _service.GetExecutionDetailsAsync(id, ct);
        return execution is null ? NotFound() : View(execution);
    }
}
