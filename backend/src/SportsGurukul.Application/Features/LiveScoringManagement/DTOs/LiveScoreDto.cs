namespace SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

public class LiveScoreDto
{
    public Guid MatchId { get; set; }
    public Guid LiveMatchId { get; set; }
    public string SportCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid HomeParticipantId { get; set; }
    public string HomeParticipantName { get; set; } = string.Empty;
    public Guid AwayParticipantId { get; set; }
    public string AwayParticipantName { get; set; } = string.Empty;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public int HomeSets { get; set; }
    public int AwaySets { get; set; }
    public int HomeGames { get; set; }
    public int AwayGames { get; set; }
    public int CurrentPeriod { get; set; }
    public string? CurrentPeriodName { get; set; }
    public DateTime? StartedAt { get; set; }
    public TimeSpan TotalPlayTime { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public int Version { get; set; }
    public List<ScoreEventDto> Events { get; set; } = new();
}

public class ScoreEventDto
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public int Points { get; set; }
    public int PeriodNumber { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsUndo { get; set; }
}
