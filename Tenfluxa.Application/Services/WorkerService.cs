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

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required");

        var worker = new Worker
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name,
            Email = request.Email,
            IsAvailable = true
        };

        await _workerRepository.AddAsync(worker);
        await _workerRepository.SaveChangesAsync();

        return worker.Id;
    }
}