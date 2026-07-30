using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class InvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
