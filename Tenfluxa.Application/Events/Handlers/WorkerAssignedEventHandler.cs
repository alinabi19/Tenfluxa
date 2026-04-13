using Microsoft.Extensions.Logging;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Events;

namespace Tenfluxa.Application.Events.Handlers;

public class WorkerAssignedEventHandler : IDomainEventHandler<WorkerAssignedEvent>
{
    private readonly ILogger<WorkerAssignedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public WorkerAssignedEventHandler(
        ILogger<WorkerAssignedEventHandler> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task HandleAsync(WorkerAssignedEvent domainEvent)
    {
        _logger.LogInformation(
            "Handling WorkerAssignedEvent → JobId: {JobId}, WorkerId: {WorkerId}",
            domainEvent.JobId,
            domainEvent.WorkerId);

        await _notificationService.NotifyJobAssignedAsync(
            domainEvent.JobId,
            domainEvent.WorkerId);


        // FUTURE:
        // send email
        // push notification
        // analytics update
    }
}