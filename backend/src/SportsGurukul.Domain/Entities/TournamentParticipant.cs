using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentParticipant : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
    public TournamentParticipantType ParticipantType { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? TeamId { get; set; }
    public Guid? AcademyId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? SeedNumber { get; set; }
    public int? Ranking { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentCategory? Category { get; set; }
    public Athlete? Athlete { get; set; }
    public TournamentTeam? Team { get; set; }
    public Academy? Academy { get; set; }
    public ICollection<TournamentMatch> HomeMatches { get; set; } = new List<TournamentMatch>();
    public ICollection<TournamentMatch> AwayMatches { get; set; } = new List<TournamentMatch>();
    public ICollection<TournamentMatch> WonMatches { get; set; } = new List<TournamentMatch>();
    public ICollection<TournamentSeed> Seeds { get; set; } = new List<TournamentSeed>();
}
