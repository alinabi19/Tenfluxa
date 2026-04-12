using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;

namespace Tenfluxa.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkerController : ControllerBase
{
    private readonly IWorkerService _workerService;

    public WorkerController(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorker(CreateWorkerRequest request, [FromQuery] Guid tenantId)
    {
        var workerId = await _workerService.CreateWorkerAsync(request, tenantId);

        return Ok(workerId);
    }
}