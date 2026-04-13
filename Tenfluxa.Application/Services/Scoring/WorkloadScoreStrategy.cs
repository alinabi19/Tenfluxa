using Tenfluxa.Application.Interfaces;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Services.Scoring;

public class WorkloadScoreStrategy : IWorkerScoringStrategy
{
    public int CalculateScore(Worker worker)
    {
        // Lower workload = better
        return Math.Max(0, 50 - worker.TotalJobsCompleted);
    }
}