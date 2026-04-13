namespace Tenfluxa.Application.Interfaces;

public interface INotificationService
{
    Task NotifyJobAssignedAsync(Guid jobId, Guid workerId);
}