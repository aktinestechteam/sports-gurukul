using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Tournament : BaseEntity
{
    public string TournamentCode { get; set; } = string.Empty;
    public string TournamentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AcademyId { get; set; }
    public Guid SportId { get; set; }
    public TournamentType TournamentType { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationOpenDate { get; set; }
    public DateTime RegistrationCloseDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int? MinParticipants { get; set; }
    public decimal? RegistrationFee { get; set; }
    public RegistrationType RegistrationType { get; set; }
    public string? Venue { get; set; }
    public string? Rules { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public bool IsPublished { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Academy Academy { get; set; } = null!;
    public Sport Sport { get; set; } = null!;
    public ICollection<TournamentCategory> Categories { get; set; } = new List<TournamentCategory>();
    public ICollection<TournamentSport> TournamentSports { get; set; } = new List<TournamentSport>();
    public ICollection<TournamentVenue> Venues { get; set; } = new List<TournamentVenue>();
    public ICollection<TournamentStage> Stages { get; set; } = new List<TournamentStage>();
    public ICollection<TournamentRegistration> Registrations { get; set; } = new List<TournamentRegistration>();
    public ICollection<TournamentParticipant> Participants { get; set; } = new List<TournamentParticipant>();
    public ICollection<TournamentTeam> Teams { get; set; } = new List<TournamentTeam>();
    public ICollection<TournamentOfficial> Officials { get; set; } = new List<TournamentOfficial>();
    public ICollection<TournamentSponsor> Sponsors { get; set; } = new List<TournamentSponsor>();
    public ICollection<TournamentDocument> Documents { get; set; } = new List<TournamentDocument>();
    public ICollection<TournamentGallery> Gallery { get; set; } = new List<TournamentGallery>();
    public ICollection<TournamentRule> Rules_ { get; set; } = new List<TournamentRule>();
    public ICollection<TournamentRanking> Rankings { get; set; } = new List<TournamentRanking>();
    public ICollection<TournamentAward> Awards { get; set; } = new List<TournamentAward>();
}
