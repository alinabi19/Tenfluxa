using Tenfluxa.Domain.Common;
using Tenfluxa.Domain.Enums;

namespace Tenfluxa.Domain.Entities;

public class Job : BaseEntity
{
    private string _title = string.Empty;
    private string _description = string.Empty;

    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Job title cannot be empty");

            _title = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Job description cannot be empty");

            _description = value;
        }
    }

    public Guid? AssignedWorkerId { get; private set; }

    public Worker? AssignedWorker { get; private set; }

    public JobStatus Status { get; private set; } = JobStatus.Pending;

    public void AssignWorker(Guid workerId)
    {
        if (workerId == Guid.Empty)
            throw new ArgumentException("Invalid worker");

        if (Status == JobStatus.Completed)
            throw new InvalidOperationException("Cannot assign worker to completed job");

        AssignedWorkerId = workerId;
        Status = JobStatus.InProgress;
    }

    public void MarkAsCompleted()
    {
        if (AssignedWorkerId == null)
            throw new InvalidOperationException("Cannot complete unassigned job");

        if (Status == JobStatus.Completed)
            throw new InvalidOperationException("Job already completed");

        Status = JobStatus.Completed;
    }
}