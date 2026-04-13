namespace Tenfluxa.Domain.Events;

public class WorkerAssignedEvent : BaseDomainEvent
{
    public Guid JobId { get; set; }
    public Guid WorkerId { get; set; }

    public WorkerAssignedEvent(Guid jobId, Guid workerId)
    {
        JobId = jobId;
        WorkerId = workerId;
    }
}