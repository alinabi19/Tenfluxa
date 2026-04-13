using Tenfluxa.Domain.Events;

namespace Tenfluxa.Application.Interfaces;

public interface IDomainEventPublisher
{
    Task PublishAsync(IEnumerable<BaseDomainEvent> events);
}