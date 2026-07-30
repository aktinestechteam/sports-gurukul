using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class SettlementBatch : BaseEntity
{
    public string BatchNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public SettlementStatus Status { get; set; } = SettlementStatus.Pending;
    public DateTime? SettledAt { get; set; }

    public ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
}
