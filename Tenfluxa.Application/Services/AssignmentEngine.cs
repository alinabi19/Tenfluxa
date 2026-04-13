using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Services;

public class AssignmentEngine : IAssignmentEngine
{
    private readonly IWorkerRepository _workerRepository;

    public AssignmentEngine(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository;
    }

    public async Task<Guid?> GetBestWorkerAsync(Guid jobId)
    {
        var workers = await _workerRepository.GetAvailableWorkersAsync();

        if (!workers.Any())
            return null;

        // SIMPLE AI LOGIC (rule-based for now)
        // Later replace with ML

        var bestWorker = workers
            .OrderByDescending(w => w.IsAvailable)
            .ThenBy(w => w.CreatedAt)
            .FirstOrDefault();

        return bestWorker?.Id;
    }
}