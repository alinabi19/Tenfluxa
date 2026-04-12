using Microsoft.AspNetCore.Mvc;
using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

[Authorize]
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
        [FromBody] CreateJobRequest request)
    {
        var result = await _jobService.CreateJobAsync(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetJobs()
    {
        var jobs = await _jobService.GetJobsAsync();
        return Ok(jobs);
    }

    [HttpPost("{jobId}/assign/{workerId}")]
    public async Task<IActionResult> AssignWorker(
        Guid jobId,
        Guid workerId)
    {
        await _jobService.AssignWorkerAsync(jobId, workerId);
        return Ok("Worker assigned successfully");
    }

    [HttpPost("{jobId}/complete")]
    public async Task<IActionResult> CompleteJob(
        Guid jobId)
    {
        await _jobService.MarkJobAsCompletedAsync(jobId);
        return Ok("Job completed successfully");
    }
}