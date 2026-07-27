using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TournamentSeed : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid ParticipantId { get; set; }
    public int SeedPosition { get; set; }
    public int? PreviousRanking { get; set; }
    public string? SeedSource { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentCategory? Category { get; set; }
    public TournamentParticipant Participant { get; set; } = null!;
}
