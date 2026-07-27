namespace SportsGurukul.Application.Features.TournamentManagement.DTOs;

public class RankingDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int Rank { get; set; }
    public int Points { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int MatchesPlayed { get; set; }
    public int SetsWon { get; set; }
    public int SetsLost { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
}
