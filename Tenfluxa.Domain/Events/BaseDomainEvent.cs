namespace Tenfluxa.Domain.Events;

public abstract class BaseDomainEvent
{
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
}