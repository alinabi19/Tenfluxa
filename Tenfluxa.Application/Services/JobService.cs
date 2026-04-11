using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;
using Tenfluxa.Domain.Enums;

namespace Tenfluxa.Application.Services;

public class JobService : IJobService
{
    private static readonly List<Job> _jobs = new();

    public Task<JobDto> CreateJobAsync(CreateJobRequest request, Guid tenantId)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = request.Title,
            Description = request.Description
        };

        _jobs.Add(job);

        var result = MapToDto(job);

        return Task.FromResult(result);
    }

    public Task<IEnumerable<JobDto>> GetJobsAsync(Guid tenantId)
    {
        var jobs = _jobs
            .Where(j => j.TenantId == tenantId)
            .Select(j => MapToDto(j));

        return Task.FromResult(jobs);
    }

    public Task AssignWorkerAsync(Guid jobId, Guid workerId, Guid tenantId)
    {
        var job = _jobs.FirstOrDefault(j => j.Id == jobId && j.TenantId == tenantId);

        if (job == null)
            throw new Exception("Job not found");

        job.AssignWorker(workerId);

        return Task.CompletedTask;
    }

    public Task MarkJobAsCompletedAsync(Guid jobId, Guid tenantId)
    {
        var job = _jobs.FirstOrDefault(j => j.Id == jobId && j.TenantId == tenantId);

        if (job == null)
            throw new Exception("Job not found");

        job.MarkAsCompleted();

        return Task.CompletedTask;
    }

    // ======================
    // Mapping
    // ======================
    private static JobDto MapToDto(Job job)
    {
        return new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Status = job.Status.ToString(),
            AssignedWorkerId = job.AssignedWorkerId
        };
    }
}