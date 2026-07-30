using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class Receipt : BaseEntity
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string? Notes { get; set; }

    public Payment Payment { get; set; } = null!;
}
