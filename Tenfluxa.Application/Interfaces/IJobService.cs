using Tenfluxa.Application.DTOs;

namespace Tenfluxa.Application.Interfaces;

public interface IJobService
{
    Task<JobDto> CreateJobAsync(CreateJobRequest request, Guid tenantId);

    Task<List<JobDto>> GetJobsAsync(Guid tenantId);

    Task AssignWorkerAsync(Guid jobId, Guid workerId, Guid tenantId);

    Task MarkJobAsCompletedAsync(Guid jobId, Guid tenantId);
}