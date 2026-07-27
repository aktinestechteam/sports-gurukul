namespace SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

public class MatchStatisticsDto
{
    public Guid MatchId { get; set; }
    public string SportCode { get; set; } = string.Empty;
    public ParticipantStatisticsDto HomeStatistics { get; set; } = new();
    public ParticipantStatisticsDto AwayStatistics { get; set; } = new();
    public List<PeriodStatisticsDto> PeriodStats { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public int TotalEvents { get; set; }
    public List<string> KeyHighlights { get; set; } = new();
}

public class ParticipantStatisticsDto
{
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int PointsPerPeriod { get; set; }
    public int Fouls { get; set; }
    public int Timeouts { get; set; }
    public int Substitutions { get; set; }
    public double AverageScore { get; set; }
    public int PossessionPercentage { get; set; }
}

public class PeriodStatisticsDto
{
    public int PeriodNumber { get; set; }
    public string? PeriodName { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public TimeSpan Duration { get; set; }
    public int HomeEvents { get; set; }
    public int AwayEvents { get; set; }
}
