using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentTeam : BaseEntity
{
    public Guid TournamentId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? TeamCode { get; set; }
    public Guid? AcademyId { get; set; }
    public int? SeedNumber { get; set; }
    public int? Ranking { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public Academy? Academy { get; set; }
    public ICollection<TournamentParticipant> Participants { get; set; } = new List<TournamentParticipant>();
    public ICollection<TournamentRegistration> Registrations { get; set; } = new List<TournamentRegistration>();
}
