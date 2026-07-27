using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TournamentRule : BaseEntity
{
    public Guid TournamentId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string RuleDescription { get; set; } = string.Empty;
    public int RuleOrder { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
}
