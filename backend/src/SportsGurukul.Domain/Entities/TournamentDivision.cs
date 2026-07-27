using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentDivision : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MaxTeams { get; set; }
    public int? MinTeams { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentCategory? Category { get; set; }
    public ICollection<TournamentRegistration> Registrations { get; set; } = new List<TournamentRegistration>();
    public ICollection<TournamentBracket> Brackets { get; set; } = new List<TournamentBracket>();
}
