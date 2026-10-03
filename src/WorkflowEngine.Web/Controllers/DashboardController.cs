using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Application.Abstractions;

namespace WorkflowEngine.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IWorkflowService _service;

    public DashboardController(IWorkflowService service) => _service = service;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var stats = await _service.GetDashboardStatsAsync(ct);
        return View(stats);
    }
}
