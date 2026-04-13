using Microsoft.Extensions.Logging;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Application.Events.Handlers;
using Tenfluxa.Domain.Events;

namespace Tenfluxa.Infrastructure.Events;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly ILogger<DomainEventPublisher> _logger;
    private readonly WorkerAssignedEventHandler _workerAssignedHandler;

    public DomainEventPublisher(
        ILogger<DomainEventPublisher> logger,
        WorkerAssignedEventHandler workerAssignedHandler)
    {
        _logger = logger;
        _workerAssignedHandler = workerAssignedHandler;
    }

    public async Task PublishAsync(IEnumerable<BaseDomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            _logger.LogInformation("Publishing event: {EventName}", domainEvent.GetType().Name);

            switch (domainEvent)
            {
                case WorkerAssignedEvent workerAssignedEvent:
                    await _workerAssignedHandler.HandleAsync(workerAssignedEvent);
                    break;

                default:
                    _logger.LogWarning("No handler found for event {EventName}", domainEvent.GetType().Name);
                    break;
            }
        }
    }
}