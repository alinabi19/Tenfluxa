using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Interfaces;

public interface IJobRepository
{
    Task AddAsync(Job job);

    Task<List<Job>> GetByTenantIdAsync(Guid tenantId);

    Task<Job?> GetByIdAsync(Guid jobId);

    Task SaveChangesAsync();
}