using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class LedgerEntry : BaseEntity
{
    public Guid LedgerId { get; set; }
    public DateTime EntryDate { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public Guid? ReferenceId { get; set; }

    public Ledger Ledger { get; set; } = null!;
}
