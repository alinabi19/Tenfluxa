using Tenfluxa.Domain.Common;
using Tenfluxa.Domain.Enums;
using Tenfluxa.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tenfluxa.Domain.Entities;

public class Job : BaseEntity
{
    private string _title = string.Empty;
    private string _description = string.Empty;

    [NotMapped]
    private readonly List<BaseDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Job title cannot be empty");

            _title = value.Trim();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Job description cannot be empty");

            _description = value.Trim();
        }
    }

    public Guid? AssignedWorkerId { get; private set; }

    public Worker? AssignedWorker { get; private set; }

    public JobStatus Status { get; private set; } = JobStatus.Pending;

    // ======================
    // Domain Methods
    // ======================

    public void AssignWorker(Guid workerId)
    {
        if (workerId == Guid.Empty)
            throw new ArgumentException("Invalid worker");

        if (Status == JobStatus.Completed)
            throw new InvalidOperationException("Cannot assign worker to completed job");

        if (AssignedWorkerId == workerId)
            throw new InvalidOperationException("Worker already assigned");

        if (AssignedWorkerId != null)
            throw new InvalidOperationException("Another worker already assigned");

        AssignedWorkerId = workerId;
        Status = JobStatus.InProgress;

        // Raise domain event
        AddDomainEvent(new WorkerAssignedEvent(this.Id, workerId));
    }

    public void MarkAsCompleted()
    {
        if (AssignedWorkerId == null)
            throw new InvalidOperationException("Cannot complete unassigned job");

        if (Status == JobStatus.Completed)
            throw new InvalidOperationException("Job already completed");

        Status = JobStatus.Completed;

        // (Optional for later)
        // AddDomainEvent(new JobCompletedEvent(this.Id));
    }

    // ======================
    // Domain Events
    // ======================

    public void AddDomainEvent(BaseDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}