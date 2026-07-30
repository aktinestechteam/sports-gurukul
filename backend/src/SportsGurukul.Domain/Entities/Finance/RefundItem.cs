using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class RefundItem : BaseEntity
{
    public Guid RefundId { get; set; }
    public Guid? InvoiceItemId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

    public Refund Refund { get; set; } = null!;
}
