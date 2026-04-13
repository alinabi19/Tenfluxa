using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Tenfluxa.Application.Services;

public class AssignmentEngine : IAssignmentEngine
{
    private readonly IWorkerRepository _workerRepository;
    private readonly IEnumerable<IWorkerScoringStrategy> _strategies;
    private readonly ILogger<AssignmentEngine> _logger;

    public AssignmentEngine(
        IWorkerRepository workerRepository,
        IEnumerable<IWorkerScoringStrategy> strategies,
        ILogger<AssignmentEngine> logger)
    {
        _workerRepository = workerRepository;
        _strategies = strategies;
        _logger = logger;
    }

    public async Task<Guid?> GetBestWorkerAsync(Guid jobId)
    {
        var workers = await _workerRepository.GetAvailableWorkersAsync();

        if (!workers.Any())
            return null;

        var scored = workers.Select(w =>
        {
            var score = _strategies.Sum(s => s.CalculateScore(w));

            _logger.LogInformation("Worker {Id} Score {Score}", w.Id, score);

            return new { Worker = w, Score = score };
        })
        .OrderByDescending(x => x.Score)
        .ToList();

        return scored.First().Worker.Id;
    }
}