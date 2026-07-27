using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Models;

public class MedalTable
{
    public Guid TournamentId { get; set; }
    public List<MedalEntry> Entries { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}
