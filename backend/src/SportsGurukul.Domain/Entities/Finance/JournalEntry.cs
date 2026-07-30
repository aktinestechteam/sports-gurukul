using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class JournalEntry : BaseEntity
{
    public Guid JournalId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }

    public Journal Journal { get; set; } = null!;
}
