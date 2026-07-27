using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TournamentResult : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid MatchId { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? ResultDetails { get; set; }
    public bool IsVerified { get; set; }
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentMatch Match { get; set; } = null!;
    public TournamentParticipant? Winner { get; set; }
}
