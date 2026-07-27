namespace SportsGurukul.Platform.Competition.Models;

public class MatchStatistics
{
    public Guid MatchId { get; set; }
    public string SportCode { get; set; } = string.Empty;
    public ParticipantStatistics HomeStatistics { get; set; } = new();
    public ParticipantStatistics AwayStatistics { get; set; } = new();
    public List<PeriodStatistics> PeriodStats { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public int TotalEvents { get; set; }
    public List<string> KeyHighlights { get; set; } = new();
}

public class ParticipantStatistics
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

public class PeriodStatistics
{
    public int PeriodNumber { get; set; }
    public string? PeriodName { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public TimeSpan Duration { get; set; }
    public int HomeEvents { get; set; }
    public int AwayEvents { get; set; }
}
