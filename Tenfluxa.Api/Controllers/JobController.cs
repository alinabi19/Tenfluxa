using Microsoft.AspNetCore.Mvc;
using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;

namespace Tenfluxa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
    {
        var tenantId = Guid.NewGuid(); // temp (i'll improve later)

        var result = await _jobService.CreateJobAsync(request, tenantId);
        return Ok(result);
    }

    [HttpGet("{tenantId}")]
    public async Task<IActionResult> GetJobs(Guid tenantId)
    {
        var jobs = await _jobService.GetJobsAsync(tenantId);
        return Ok(jobs);
    }

    [HttpPost("{jobId}/assign/{workerId}")]
    public async Task<IActionResult> AssignWorker(Guid jobId, Guid workerId, Guid tenantId)
    {
        await _jobService.AssignWorkerAsync(jobId, workerId, tenantId);
        return Ok();
    }

    [HttpPost("{jobId}/complete")]
    public async Task<IActionResult> CompleteJob(Guid jobId, Guid tenantId)
    {
        await _jobService.MarkJobAsCompletedAsync(jobId, tenantId);
        return Ok();
    }
}