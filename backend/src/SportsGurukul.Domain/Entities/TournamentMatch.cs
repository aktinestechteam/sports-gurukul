using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentMatch : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? TournamentStageId { get; set; }
    public Guid? TournamentRoundId { get; set; }
    public Guid? TournamentVenueId { get; set; }
    public Guid? TournamentCourtId { get; set; }
    public int MatchNumber { get; set; }
    public Guid? HomeParticipantId { get; set; }
    public Guid? AwayParticipantId { get; set; }
    public string? HomeParticipantName { get; set; }
    public string? AwayParticipantName { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public TimeSpan? ScheduledTime { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Scheduled;
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? ScoreDetails { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentStage? TournamentStage { get; set; }
    public TournamentRound? TournamentRound { get; set; }
    public TournamentVenue? TournamentVenue { get; set; }
    public TournamentCourt? TournamentCourt { get; set; }
    public TournamentParticipant? HomeParticipant { get; set; }
    public TournamentParticipant? AwayParticipant { get; set; }
    public TournamentParticipant? Winner { get; set; }
    public ICollection<TournamentMatchSet> Sets { get; set; } = new List<TournamentMatchSet>();
    public ICollection<TournamentResult> Results { get; set; } = new List<TournamentResult>();
}
