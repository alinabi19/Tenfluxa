using Tenfluxa.Domain.Common;
using Tenfluxa.Domain.Enums;

namespace Tenfluxa.Domain.Entities;

public class Job : BaseEntity
{
    private string _title = string.Empty;
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

    private string _description = string.Empty;
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

    public void AssignWorker(Guid workerId)
    {
        if (workerId == Guid.Empty)
            throw new ArgumentException("Invalid worker");

        AssignedWorkerId = workerId;
        Status = JobStatus.InProgress;
    }

    public void MarkAsCompleted()
    {
        if (AssignedWorkerId == null)
            throw new InvalidOperationException("Cannot complete unassigned job");

        Status = JobStatus.Completed;
    }

    public Guid? AssignedWorkerId { get; set; }

    public Worker? AssignedWorker { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Pending;
}