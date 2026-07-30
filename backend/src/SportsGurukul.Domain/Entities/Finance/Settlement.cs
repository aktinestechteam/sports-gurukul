using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class Settlement : BaseEntity
{
    public Guid SettlementBatchId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public SettlementStatus Status { get; set; } = SettlementStatus.Pending;
    public DateTime? SettledAt { get; set; }
    public string? Reference { get; set; }

    public SettlementBatch SettlementBatch { get; set; } = null!;
    public Payment Payment { get; set; } = null!;
}
