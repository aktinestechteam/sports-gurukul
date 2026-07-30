using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class InvoiceTax : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
