
using Microsoft.EntityFrameworkCore;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Infrastructure.Persistence.Repositories;

public class WorkerRepository : IWorkerRepository
{
    private readonly TenfluxaDbContext _context;

    public WorkerRepository(TenfluxaDbContext context)
    {
        _context = context;
    }

    public async Task<Worker?> GetByIdAsync(Guid workerId)
    {
        return await _context.Workers
            .FirstOrDefaultAsync(w => w.Id == workerId);
    }

    public async Task<Worker?> GetByEmailAsync(string email, Guid tenantId)
    {
        return await _context.Workers
            .FirstOrDefaultAsync(w => w.Email == email && w.TenantId == tenantId);
    }

    public async Task AddAsync(Worker worker)
    {
        await _context.Workers.AddAsync(worker);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}