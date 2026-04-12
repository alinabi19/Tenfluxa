using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Services;

public class WorkerService : IWorkerService
{
    private readonly IWorkerRepository _workerRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<WorkerService> _logger;

    public WorkerService(
        IWorkerRepository workerRepository,
        ITenantProvider tenantProvider,
        ILogger<WorkerService> logger)
    {
        _workerRepository = workerRepository;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public async Task<Guid> CreateWorkerAsync(CreateWorkerRequest request)
    {
        var tenantId = _tenantProvider.GetTenantId();

        // Validate tenant first
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Creating worker for tenant {TenantId}", tenantId);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("CreateWorker failed: Name is empty");
            throw new ArgumentException("Name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("CreateWorker failed: Email is empty");
            throw new ArgumentException("Email is required");
        }

        var name = request.Name.Trim();
        var email = request.Email.Trim().ToLower();

        if (name.Length > 100)
        {
            _logger.LogWarning("CreateWorker failed: Name too long");
            throw new ArgumentException("Name too long");
        }

        if (email.Length > 200)
        {
            _logger.LogWarning("CreateWorker failed: Email too long");
            throw new ArgumentException("Email too long");
        }

        // Email format validation
        if (!IsValidEmail(email))
        {
            _logger.LogWarning("CreateWorker failed: Invalid email format {Email}", email);
            throw new ArgumentException("Invalid email format");
        }

        // Check duplicate
        var existingWorker = await _workerRepository.GetByEmailAsync(email, tenantId);
        if (existingWorker != null)
        {
            _logger.LogWarning("Worker with email {Email} already exists for tenant {TenantId}", email, tenantId);
            throw new InvalidOperationException("Worker with this email already exists");
        }

        var worker = new Worker
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Email = email,
            IsAvailable = true
        };

        await _workerRepository.AddAsync(worker);
        await _workerRepository.SaveChangesAsync();

        _logger.LogInformation("Worker created successfully with id {WorkerId}", worker.Id);

        return worker.Id;
    }

    // ======================
    // Helpers
    // ======================
    private static bool IsValidEmail(string email)
    {
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email);
    }
}