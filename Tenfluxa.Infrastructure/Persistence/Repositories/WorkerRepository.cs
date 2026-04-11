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

    public async Task AddAsync(Worker worker)
    {
        await _context.Workers.AddAsync(worker);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}