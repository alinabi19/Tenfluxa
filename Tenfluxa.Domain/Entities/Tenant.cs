using Tenfluxa.Domain.Common;

namespace Tenfluxa.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Identifier { get; set; } = string.Empty;
}