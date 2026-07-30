using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class CreditNote : BaseEntity
{
    public string CreditNoteNumber { get; set; } = string.Empty;
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public CreditNoteStatus Status { get; set; } = CreditNoteStatus.Draft;
    public DateTime IssuedAt { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
