using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;
using Tenfluxa.Domain.Enums;

namespace Tenfluxa.Application.Services;

public class JobService : IJobService
{
    private static readonly List<Job> _jobs = new();

    public async Task<JobDto> CreateJobAsync(CreateJobRequest request, Guid tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description is required");

        var job = new Job
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = request.Title,
            Description = request.Description
        };

        _jobs.Add(job);

        return MapToDto(job);
    }

    public async Task<List<JobDto>> GetJobsAsync(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        return _jobs
            .Where(j => j.TenantId == tenantId)
            .Select(MapToDto)
            .ToList();
    }

    public async Task AssignWorkerAsync(Guid jobId, Guid workerId, Guid tenantId)
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("Invalid jobId");

        if (workerId == Guid.Empty)
            throw new ArgumentException("Invalid workerId");

        var job = _jobs.FirstOrDefault(j => j.Id == jobId && j.TenantId == tenantId);

        if (job == null)
            throw new Exception("Job not found");

        job.AssignWorker(workerId);
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