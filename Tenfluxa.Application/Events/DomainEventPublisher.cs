using Microsoft.Extensions.Logging;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Events;

namespace Tenfluxa.Infrastructure.Events;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly ILogger<DomainEventPublisher> _logger;

    public DomainEventPublisher(ILogger<DomainEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(IEnumerable<BaseDomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            _logger.LogInformation("Publishing event: {EventName}", domainEvent.GetType().Name);
        }

        return Task.CompletedTask;
    }
}