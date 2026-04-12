using Tenfluxa.Application.DTOs;

namespace Tenfluxa.Application.Interfaces;

public interface IWorkerService
{
    Task<Guid> CreateWorkerAsync(CreateWorkerRequest request);
}