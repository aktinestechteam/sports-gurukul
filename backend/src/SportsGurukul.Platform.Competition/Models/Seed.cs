namespace SportsGurukul.Platform.Competition.Models;

public class Seed
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public int Position { get; set; }
    public Guid ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public string? SeedNumber { get; set; }
    public string? Region { get; set; }
    public Guid? AcademyId { get; set; }
    public int? CurrentRanking { get; set; }
}
