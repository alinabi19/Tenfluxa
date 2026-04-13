using Tenfluxa.Application.DTOs;

namespace Tenfluxa.Application.Interfaces;

public interface IJobService
{
    Task<JobDto> CreateJobAsync(CreateJobRequest request);

    Task<List<JobDto>> GetJobsAsync();

    Task AssignWorkerAsync(Guid jobId, Guid workerId);

    Task MarkJobAsCompletedAsync(Guid jobId);

    Task AssignBestWorkerAsync(Guid jobId);
}