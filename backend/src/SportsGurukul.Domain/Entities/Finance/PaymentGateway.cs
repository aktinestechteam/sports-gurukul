using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class PaymentGateway : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? Configuration { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<GatewayTransaction> GatewayTransactions { get; set; } = new List<GatewayTransaction>();
}
