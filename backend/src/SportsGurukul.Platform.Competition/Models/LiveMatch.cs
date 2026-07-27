using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class LiveMatch
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid MatchId { get; set; }
    public string SportCode { get; set; } = string.Empty;
    public LiveMatchStatus Status { get; set; } = LiveMatchStatus.Scheduled;
    public Guid HomeParticipantId { get; set; }
    public string HomeParticipantName { get; set; } = string.Empty;
    public Guid AwayParticipantId { get; set; }
    public string AwayParticipantName { get; set; } = string.Empty;
    public MatchScore HomeScore { get; set; } = new();
    public MatchScore AwayScore { get; set; } = new();
    public int CurrentPeriod { get; set; }
    public string? CurrentPeriodName { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? PausedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan TotalPlayTime { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public string? ScoreDetails { get; set; }
    public List<LiveScoreEvent> ScoreEvents { get; set; } = new();
    public List<ParticipantScore> ParticipantScores { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
    public int Version { get; set; }
}
