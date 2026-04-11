using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;
using Tenfluxa.Domain.Enums;

namespace Tenfluxa.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IWorkerRepository _workerRepository;

    public JobService(
        IJobRepository jobRepository,
        IWorkerRepository workerRepository)
    {
        _jobRepository = jobRepository;
        _workerRepository = workerRepository;
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

        if (request.Title.Length > 200)
            throw new ArgumentException("Title too long");

        var job = new Job
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim()
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
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        if (jobId == Guid.Empty)
            throw new ArgumentException("Invalid jobId");

        if (workerId == Guid.Empty)
            throw new ArgumentException("Invalid workerId");

        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.TenantId != tenantId)
            throw new KeyNotFoundException("Job not found");

        if (job.Status == JobStatus.Completed)
            throw new InvalidOperationException("Cannot assign worker to completed job");

        if (job.AssignedWorkerId == workerId)
            throw new InvalidOperationException("Worker already assigned to this job");

        if (job.AssignedWorkerId != null)
            throw new InvalidOperationException("Another worker already assigned");

        var worker = await _workerRepository.GetByIdAsync(workerId);

        if (worker == null || worker.TenantId != tenantId)
            throw new KeyNotFoundException("Worker not found");

        if (!worker.IsAvailable)
            throw new InvalidOperationException("Worker is not available");

        // Assign worker
        job.AssignWorker(workerId);

        // Mark worker as busy
        worker.IsAvailable = false;

        await _jobRepository.SaveChangesAsync();
    }

    public async Task MarkJobAsCompletedAsync(Guid jobId, Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        if (jobId == Guid.Empty)
            throw new ArgumentException("Invalid jobId");

        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.TenantId != tenantId)
            throw new KeyNotFoundException("Job not found");

        if (job.AssignedWorkerId == null)
            throw new InvalidOperationException("Cannot complete job without assigned worker");

        if (job.Status == JobStatus.Completed)
            throw new InvalidOperationException("Job already completed");

        // Mark job complete
        job.MarkAsCompleted();

        // Free worker
        var worker = await _workerRepository.GetByIdAsync(job.AssignedWorkerId.Value);
        if (worker != null)
        {
            worker.IsAvailable = true;
        }

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