using Tenfluxa.Domain.Events;

namespace Tenfluxa.Application.Interfaces;

public interface IDomainEventHandler<T> where T : BaseDomainEvent
{
    Task HandleAsync(T domainEvent);
}