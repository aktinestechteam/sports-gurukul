using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class DebitNote : BaseEntity
{
    public string DebitNoteNumber { get; set; } = string.Empty;
    public Guid InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DebitNoteStatus Status { get; set; } = DebitNoteStatus.Draft;
    public DateTime IssuedAt { get; set; }

    public Invoice Invoice { get; set; } = null!;
}
