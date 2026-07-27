using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.DTOs;

public class MatchDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid? TournamentStageId { get; set; }
    public string? StageName { get; set; }
    public Guid? TournamentRoundId { get; set; }
    public int? RoundNumber { get; set; }
    public int MatchNumber { get; set; }
    public Guid? HomeParticipantId { get; set; }
    public string? HomeParticipantName { get; set; }
    public Guid? AwayParticipantId { get; set; }
    public string? AwayParticipantName { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public TimeSpan? ScheduledTime { get; set; }
    public string? VenueName { get; set; }
    public string? CourtName { get; set; }
    public MatchStatus Status { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? ScoreDetails { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<MatchSetDto> Sets { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class MatchSetDto
{
    public Guid Id { get; set; }
    public int SetNumber { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? SetDetails { get; set; }
    public string? WinnerName { get; set; }
}
