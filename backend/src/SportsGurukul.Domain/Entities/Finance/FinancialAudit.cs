using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class FinancialAudit : BaseEntity
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Changes { get; set; }
    public Guid? PerformedBy { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? IpAddress { get; set; }
}
