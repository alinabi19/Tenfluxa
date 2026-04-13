using Microsoft.Extensions.DependencyInjection;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Events;

namespace Tenfluxa.Infrastructure.Events;

public interface IDomainEventHandlerDispatcher
{
    Task Dispatch(BaseDomainEvent domainEvent);

    Task DispatchWorkerAssignedEvent(Guid jobId, Guid workerId);
}

public class DomainEventHandlerDispatcher : IDomainEventHandlerDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventHandlerDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Dispatch(BaseDomainEvent domainEvent)
    {
        using var scope = _serviceProvider.CreateScope();

        var handlers = scope.ServiceProvider
            .GetServices(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));

        foreach (var handler in handlers)
        {
            var method = handler.GetType().GetMethod("HandleAsync");

            if (method != null)
            {
                await (Task)method.Invoke(handler, new object[] { domainEvent });
            }
        }
    }

    public async Task DispatchWorkerAssignedEvent(Guid jobId, Guid workerId)
    {
        using var scope = _serviceProvider.CreateScope();

        var handlers = scope.ServiceProvider
            .GetServices<IDomainEventHandler<WorkerAssignedEvent>>();

        var domainEvent = new WorkerAssignedEvent(jobId, workerId);

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(domainEvent);
        }
    }
}