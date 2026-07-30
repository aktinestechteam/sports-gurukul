using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class Journal : BaseEntity
{
    public string JournalNumber { get; set; } = string.Empty;
    public DateTime JournalDate { get; set; }
    public string? Description { get; set; }
    public JournalStatus Status { get; set; } = JournalStatus.Draft;
    public string? Period { get; set; }

    public ICollection<JournalEntry> Entries { get; set; } = new List<JournalEntry>();
}
