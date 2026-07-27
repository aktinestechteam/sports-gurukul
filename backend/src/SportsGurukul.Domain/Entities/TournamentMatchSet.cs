using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentMatchSet : BaseEntity
{
    public Guid TournamentMatchId { get; set; }
    public int SetNumber { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? SetDetails { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public TournamentMatch TournamentMatch { get; set; } = null!;
}
