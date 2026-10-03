using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Application.Abstractions;
using WorkflowEngine.Domain.Enums;
using WorkflowEngine.Web.Models;

namespace WorkflowEngine.Web.Controllers;

public class WorkflowsController : Controller
{
    private readonly IWorkflowService _service;

    public WorkflowsController(IWorkflowService service) => _service = service;

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _service.GetAllAsync(ct));

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var workflow = await _service.GetDetailsAsync(id, ct);
        return workflow is null ? NotFound() : View(workflow);
    }

    public IActionResult Create() => View(new WorkflowFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkflowFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        var id = await _service.CreateAsync(model.Name, model.Description, ct);
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var workflow = await _service.GetDetailsAsync(id, ct);
        if (workflow is null) return NotFound();
        return View(new WorkflowFormViewModel
        {
            Id = workflow.Id,
            Name = workflow.Name,
            Description = workflow.Description,
            Status = workflow.Status
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(WorkflowFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.UpdateAsync(model.Id!.Value, model.Name, model.Description, model.Status, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
