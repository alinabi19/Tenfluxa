using Tenfluxa.Domain.Common;

namespace Tenfluxa.Domain.Entities;

public class Worker : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}