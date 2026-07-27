using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TournamentRanking : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid ParticipantId { get; set; }
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
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentCategory? Category { get; set; }
    public TournamentParticipant Participant { get; set; } = null!;
}
