using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<JobDto> CreateJobAsync(CreateJobRequest request, Guid tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

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

        await _jobRepository.AddAsync(job);
        await _jobRepository.SaveChangesAsync();

        return MapToDto(job);
    }

    public async Task<List<JobDto>> GetJobsAsync(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        var jobs = await _jobRepository.GetByTenantIdAsync(tenantId);

        return jobs.Select(MapToDto).ToList();
    }

    public async Task AssignWorkerAsync(Guid jobId, Guid workerId, Guid tenantId)
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("Invalid jobId");

        if (workerId == Guid.Empty)
            throw new ArgumentException("Invalid workerId");

        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.TenantId != tenantId)
            throw new Exception("Job not found");

        job.AssignWorker(workerId);

        await _jobRepository.SaveChangesAsync();
    }

    public async Task MarkJobAsCompletedAsync(Guid jobId, Guid tenantId)
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("Invalid jobId");

        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.TenantId != tenantId)
            throw new Exception("Job not found");

        job.MarkAsCompleted();

        await _jobRepository.SaveChangesAsync();
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