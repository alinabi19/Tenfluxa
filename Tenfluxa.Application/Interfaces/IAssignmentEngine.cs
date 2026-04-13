namespace Tenfluxa.Application.Interfaces;

public interface IAssignmentEngine
{
    Task<Guid?> GetBestWorkerAsync(Guid jobId);
}