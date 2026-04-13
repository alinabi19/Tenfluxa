using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;
using Tenfluxa.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Tenfluxa.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<JobService> _logger;
    private readonly IDomainEventPublisher _eventPublisher;

    public JobService(
        IJobRepository jobRepository,
        IWorkerRepository workerRepository,
        ITenantProvider tenantProvider,
        ILogger<JobService> logger,
        IDomainEventPublisher eventPublisher)
    {
        _jobRepository = jobRepository;
        _workerRepository = workerRepository;
        _tenantProvider = tenantProvider;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task<JobDto> CreateJobAsync(CreateJobRequest request)
    {
        var tenantId = _tenantProvider.GetTenantId();

        // Validate tenant first
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            _logger.LogWarning("CreateJob failed: Title is empty");
            throw new ArgumentException("Title is required");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            _logger.LogWarning("CreateJob failed: Description is empty");
            throw new ArgumentException("Description is required");
        }

        if (request.Title.Length > 200)
        {
            _logger.LogWarning("CreateJob failed: Title too long");
            throw new ArgumentException("Title too long");
        }

        _logger.LogInformation("Creating job for tenant {TenantId}", tenantId);

        var job = new Job
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim()
        };

        await _jobRepository.AddAsync(job);
        await _jobRepository.SaveChangesAsync();

        _logger.LogInformation("Job created successfully with id {JobId}", job.Id);

        return MapToDto(job);
    }

    public async Task<List<JobDto>> GetJobsAsync()
    {
        var tenantId = _tenantProvider.GetTenantId();

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        _logger.LogInformation("Retrieving jobs for tenant {TenantId}", tenantId);

        var jobs = await _jobRepository.GetByTenantIdAsync(tenantId);

        return jobs.Select(MapToDto).ToList();
    }

    public async Task AssignWorkerAsync(Guid jobId, Guid workerId)
    {
        var tenantId = _tenantProvider.GetTenantId();

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        if (jobId == Guid.Empty)
            throw new ArgumentException("Invalid jobId");

        if (workerId == Guid.Empty)
            throw new ArgumentException("Invalid workerId");

        _logger.LogInformation("Assigning worker {WorkerId} to job {JobId}", workerId, jobId);

        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.TenantId != tenantId)
        {
            _logger.LogError("Job not found for id {JobId}", jobId);
            throw new KeyNotFoundException("Job not found");
        }

        if (job.Status == JobStatus.Completed)
        {
            _logger.LogWarning("Attempt to assign worker to completed job {JobId}", jobId);
            throw new InvalidOperationException("Cannot assign worker to completed job");
        }

        if (job.AssignedWorkerId == workerId)
        {
            _logger.LogWarning("Worker {WorkerId} already assigned to job {JobId}", workerId, jobId);
            throw new InvalidOperationException("Worker already assigned to this job");
        }

        if (job.AssignedWorkerId != null)
        {
            _logger.LogWarning("Another worker already assigned to job {JobId}", jobId);
            throw new InvalidOperationException("Another worker already assigned");
        }

        var worker = await _workerRepository.GetByIdAsync(workerId);

        if (worker == null || worker.TenantId != tenantId)
        {
            _logger.LogError("Worker not found for id {WorkerId}", workerId);
            throw new KeyNotFoundException("Worker not found");
        }

        if (!worker.IsAvailable)
        {
            _logger.LogWarning("Worker {WorkerId} is not available", workerId);
            throw new InvalidOperationException("Worker is not available");
        }

        // Assign worker
        job.AssignWorker(workerId);

        // Mark worker busy
        worker.IsAvailable = false;

        await _jobRepository.SaveChangesAsync();


        // Publish domain events
        await _eventPublisher.PublishAsync(job.DomainEvents);
        job.ClearDomainEvents();

        // Log success AFTER everything
        _logger.LogInformation("Worker {WorkerId} assigned successfully to job {JobId}", workerId, jobId);
    }

    public async Task MarkJobAsCompletedAsync(Guid jobId)
    {
        var tenantId = _tenantProvider.GetTenantId();

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        if (jobId == Guid.Empty)
            throw new ArgumentException("Invalid jobId");

        _logger.LogInformation("Marking job {JobId} as completed", jobId);

        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.TenantId != tenantId)
        {
            _logger.LogError("Job not found for id {JobId}", jobId);
            throw new KeyNotFoundException("Job not found");
        }

        if (job.AssignedWorkerId == null)
        {
            _logger.LogWarning("Cannot complete job {JobId} without assigned worker", jobId);
            throw new InvalidOperationException("Cannot complete job without assigned worker");
        }

        if (job.Status == JobStatus.Completed)
        {
            _logger.LogWarning("Job {JobId} is already completed", jobId);
            throw new InvalidOperationException("Job already completed");
        }

        // Mark complete
        job.MarkAsCompleted();

        // Free worker
        var worker = await _workerRepository.GetByIdAsync(job.AssignedWorkerId.Value);
        if (worker != null)
        {
            worker.IsAvailable = true;
        }

        await _jobRepository.SaveChangesAsync();

        _logger.LogInformation("Job {JobId} marked as completed successfully", jobId);
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