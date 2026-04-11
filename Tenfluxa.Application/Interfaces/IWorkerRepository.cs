using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Interfaces;

public interface IWorkerRepository
{
    Task AddAsync(Worker worker);
    Task SaveChangesAsync();
}