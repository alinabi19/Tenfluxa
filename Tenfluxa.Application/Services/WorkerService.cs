using System.Text.RegularExpressions;
using Tenfluxa.Application.DTOs;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Services;

public class WorkerService : IWorkerService
{
    private readonly IWorkerRepository _workerRepository;

    public WorkerService(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository;
    }

    public async Task<Guid> CreateWorkerAsync(CreateWorkerRequest request, Guid tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Invalid tenantId");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required");

        var name = request.Name.Trim();
        var email = request.Email.Trim().ToLower();

        if (name.Length > 100)
            throw new ArgumentException("Name too long");

        if (email.Length > 200)
            throw new ArgumentException("Email too long");

        // Basic email format validation
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format");

        // prevent duplicate workers (same email per tenant)
        var existingWorker = await _workerRepository.GetByEmailAsync(email, tenantId);
        if (existingWorker != null)
            throw new InvalidOperationException("Worker with this email already exists");

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