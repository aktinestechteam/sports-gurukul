using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class InvoiceDiscount : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string DiscountName { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal DiscountAmount { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
