using Microsoft.EntityFrameworkCore;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;
using Tenfluxa.Infrastructure.Persistence;

namespace Tenfluxa.Infrastructure.Persistence.Repositories;

public class JobRepository : IJobRepository
{
    private readonly TenfluxaDbContext _context;

    public JobRepository(TenfluxaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Job job)
    {
        await _context.Jobs.AddAsync(job);
    }

    public async Task<List<Job>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _context.Jobs
            .Where(j => j.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<Job?> GetByIdAsync(Guid jobId)
    {
        return await _context.Jobs
            .FirstOrDefaultAsync(j => j.Id == jobId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}