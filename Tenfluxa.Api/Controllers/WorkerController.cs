using Microsoft.AspNetCore.Mvc;
using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;

namespace Tenfluxa.Api.Controllers;

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
    public async Task<IActionResult> CreateWorker(CreateWorkerRequest request)
    {
        var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var workerId = await _workerService.CreateWorkerAsync(request, tenantId);

        return Ok(workerId);
    }
}