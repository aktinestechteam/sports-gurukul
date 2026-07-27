namespace SportsGurukul.Platform.Competition.Models;

public class Ranking
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public Guid? CategoryId { get; set; }
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
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
}
