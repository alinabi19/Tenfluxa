using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Interfaces;

public interface IWorkerRepository
{
    Task<Worker?> GetByIdAsync(Guid workerId);
    Task<Worker?> GetByEmailAsync(string email, Guid tenantId);
    Task AddAsync(Worker worker);
    Task SaveChangesAsync();

    Task<List<Worker>> GetAvailableWorkersAsync();
}