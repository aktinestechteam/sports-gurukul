using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TournamentRegistration : BaseEntity
{
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? DivisionId { get; set; }
    public TournamentRegistrationStatus RegistrationStatus { get; set; } = TournamentRegistrationStatus.Pending;
    public Guid? AthleteId { get; set; }
    public Guid? TeamId { get; set; }
    public Guid? AcademyId { get; set; }
    public string RegistrantName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal? FeePaid { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? CheckedInDate { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Tournament Tournament { get; set; } = null!;
    public TournamentCategory? Category { get; set; }
    public TournamentDivision? Division { get; set; }
    public Athlete? Athlete { get; set; }
    public TournamentTeam? Team { get; set; }
    public Academy? Academy { get; set; }
}
