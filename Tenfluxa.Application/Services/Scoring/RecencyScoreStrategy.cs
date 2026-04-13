using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Services.Scoring;

public class RecencyScoreStrategy : IWorkerScoringStrategy
{
    public int CalculateScore(Worker worker)
    {
        var minutes = (DateTime.UtcNow - worker.LastAssignedAt).TotalMinutes;

        return (int)Math.Min(minutes, 50);
    }
}