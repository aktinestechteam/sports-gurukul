using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class GatewayTransaction : BaseEntity
{
    public Guid GatewayId { get; set; }
    public Guid PaymentId { get; set; }
    public string? TransactionId { get; set; }
    public string? RequestPayload { get; set; }
    public string? ResponsePayload { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime TransactedAt { get; set; }

    public PaymentGateway Gateway { get; set; } = null!;
    public Payment Payment { get; set; } = null!;
}
