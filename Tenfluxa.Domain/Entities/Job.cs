using Tenfluxa.Domain.Common;
using Tenfluxa.Domain.Enums;

namespace Tenfluxa.Domain.Entities;

public class Job : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid? AssignedWorkerId { get; set; }

    public Worker? AssignedWorker { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Pending;
}