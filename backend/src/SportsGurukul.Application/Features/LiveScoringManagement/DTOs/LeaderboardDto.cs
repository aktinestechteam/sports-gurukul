namespace SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

public class LeaderboardDto
{
    public Guid TournamentId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? SportCode { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<LeaderboardEntryDto> Entries { get; set; } = new();
}

public class LeaderboardEntryDto
{
    public int Position { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? AcademyName { get; set; }
    public int Points { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int MatchesPlayed { get; set; }
    public decimal WinPercentage { get; set; }
    public int GoalDifference { get; set; }
}
