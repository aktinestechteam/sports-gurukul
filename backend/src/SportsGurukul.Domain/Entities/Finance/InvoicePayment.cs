using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class InvoicePayment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal AmountApplied { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public Payment Payment { get; set; } = null!;
}
