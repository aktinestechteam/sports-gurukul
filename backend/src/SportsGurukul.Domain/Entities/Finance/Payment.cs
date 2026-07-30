using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class Payment : BaseEntity
{
    public string PaymentReference { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public Enums.Finance.PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? Description { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? GatewayId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? FailureReason { get; set; }
    public bool IsIdempotent { get; set; }
    public string? IdempotencyKey { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Invoice? Invoice { get; set; }
    public PaymentGateway? Gateway { get; set; }
    public ICollection<InvoicePayment> InvoicePayments { get; set; } = new List<InvoicePayment>();
    public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
    public ICollection<GatewayTransaction> GatewayTransactions { get; set; } = new List<GatewayTransaction>();
    public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    public ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
}
