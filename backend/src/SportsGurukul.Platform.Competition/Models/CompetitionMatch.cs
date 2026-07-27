using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class CompetitionMatch
{
    public Guid Id { get; set; }
    public int MatchNumber { get; set; }
    public int RoundNumber { get; set; }
    public RoundType RoundType { get; set; }
    public BracketType BracketType { get; set; } = BracketType.Main;
    public Guid? HomeParticipantId { get; set; }
    public string? HomeParticipantName { get; set; }
    public Guid? AwayParticipantId { get; set; }
    public string? AwayParticipantName { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? ScoreDetails { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Scheduled;
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public AdvancementReason? WinnerAdvancementReason { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public TimeSpan? ScheduledTime { get; set; }
    public Guid? VenueId { get; set; }
    public Guid? CourtId { get; set; }
    public Guid? OfficialId { get; set; }
    public string? Notes { get; set; }
    public bool IsBye => HomeParticipantId is null || AwayParticipantId is null;
    public bool IsCompleted => Status == MatchStatus.Completed || Status == MatchStatus.Walkover || Status == MatchStatus.Forfeit;
    public List<MatchSet> Sets { get; set; } = new();
}
