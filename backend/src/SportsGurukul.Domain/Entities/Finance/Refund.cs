using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class Refund : BaseEntity
{
    public string RefundNumber { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public RefundStatus Status { get; set; } = RefundStatus.Requested;
    public decimal TotalAmount { get; set; }
    public DateTime RefundDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? GatewayReference { get; set; }
    public string? Notes { get; set; }

    public Payment Payment { get; set; } = null!;
    public ICollection<RefundItem> RefundItems { get; set; } = new List<RefundItem>();
}
