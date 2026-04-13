using Microsoft.Extensions.Logging;
using Tenfluxa.Domain.Events;

namespace Tenfluxa.Application.Events.Handlers;

public class WorkerAssignedEventHandler
{
    private readonly ILogger<WorkerAssignedEventHandler> _logger;

    public WorkerAssignedEventHandler(ILogger<WorkerAssignedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(WorkerAssignedEvent domainEvent)
    {
        _logger.LogInformation(
            "Handling WorkerAssignedEvent → JobId: {JobId}, WorkerId: {WorkerId}",
            domainEvent.JobId,
            domainEvent.WorkerId
        );

        // FUTURE:
        // send email
        // push notification
        // analytics update

        return Task.CompletedTask;
    }
}