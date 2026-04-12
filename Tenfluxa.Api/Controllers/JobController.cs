using Microsoft.AspNetCore.Mvc;
using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;

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
    public async Task<IActionResult> CreateJob(
        [FromBody] CreateJobRequest request,
        [FromQuery] Guid tenantId)
    {
        var result = await _jobService.CreateJobAsync(request, tenantId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetJobs([FromQuery] Guid tenantId)
    {
        var jobs = await _jobService.GetJobsAsync(tenantId);
        return Ok(jobs);
    }

    [HttpPost("{jobId}/assign/{workerId}")]
    public async Task<IActionResult> AssignWorker(
        Guid jobId,
        Guid workerId,
        [FromQuery] Guid tenantId)
    {
        await _jobService.AssignWorkerAsync(jobId, workerId, tenantId);
        return Ok("Worker assigned successfully");
    }

    [HttpPost("{jobId}/complete")]
    public async Task<IActionResult> CompleteJob(
        Guid jobId,
        [FromQuery] Guid tenantId)
    {
        await _jobService.MarkJobAsCompletedAsync(jobId, tenantId);
        return Ok("Job completed successfully");
    }
}