using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentCategory : BaseEntity
{
    public Guid TournamentId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public TournamentCategoryType CategoryType { get; set; }
    public string? Description { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public Gender? Gender { get; set; }
    public int? MinSkillLevel { get; set; }
    public int? MaxSkillLevel { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public ICollection<TournamentRegistration> Registrations { get; set; } = new List<TournamentRegistration>();
    public ICollection<TournamentParticipant> Participants { get; set; } = new List<TournamentParticipant>();
    public ICollection<TournamentRanking> Rankings { get; set; } = new List<TournamentRanking>();
}
