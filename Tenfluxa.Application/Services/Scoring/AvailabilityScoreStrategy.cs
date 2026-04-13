using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Services.Scoring;

public class AvailabilityScoreStrategy : IWorkerScoringStrategy
{
    public int CalculateScore(Worker worker)
    {
        return worker.IsAvailable ? 100 : -1000;
    }
}