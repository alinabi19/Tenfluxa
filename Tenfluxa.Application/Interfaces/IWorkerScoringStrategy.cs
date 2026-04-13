using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Application.Interfaces;

public interface IWorkerScoringStrategy
{
    int CalculateScore(Worker worker);
}