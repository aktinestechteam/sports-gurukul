using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class PaymentTransaction : BaseEntity
{
    public Guid PaymentId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? GatewayResponse { get; set; }
    public string? TransactionReference { get; set; }

    public Payment Payment { get; set; } = null!;
}
