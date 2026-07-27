using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.DTOs;

public class ParticipantDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public TournamentParticipantType ParticipantType { get; set; }
    public Guid? AthleteId { get; set; }
    public string? AthleteName { get; set; }
    public Guid? TeamId { get; set; }
    public string? TeamName { get; set; }
    public Guid? AcademyId { get; set; }
    public string? AcademyName { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? SeedNumber { get; set; }
    public int? Ranking { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ParticipantStatisticsDto
{
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int MatchesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int SetsWon { get; set; }
    public int SetsLost { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int Points { get; set; }
    public int CurrentRank { get; set; }
}
