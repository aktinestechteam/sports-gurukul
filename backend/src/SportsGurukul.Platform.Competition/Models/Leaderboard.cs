using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class Leaderboard
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public LeaderboardType Type { get; set; }
    public string? SportCode { get; set; }
    public string? SeasonCode { get; set; }
    public List<LeaderboardEntry> Entries { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public int Version { get; set; }
}
