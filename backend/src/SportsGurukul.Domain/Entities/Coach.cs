using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Coach : BaseEntity
{
    public Guid UserId { get; set; }
    public string CoachCode { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string? Biography { get; set; }
    public int YearsOfExperience { get; set; }
    public string? CurrentOrganization { get; set; }
    public string? HighestQualification { get; set; }
    public string? PreferredLanguage { get; set; }
    public CoachingLevel CoachingLevel { get; set; } = CoachingLevel.Junior;
    public CoachStatus Status { get; set; } = CoachStatus.Pending;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public byte[] RowVersion { get; set; } = [];

    public User User { get; set; } = null!;
    public CoachAvailability? Availability { get; set; }
    public CoachLocation? Location { get; set; }
    public ICollection<CoachSport> CoachSports { get; set; } = new List<CoachSport>();
    public ICollection<CoachCertification> Certifications { get; set; } = new List<CoachCertification>();
    public ICollection<CoachExperience> Experiences { get; set; } = new List<CoachExperience>();
    public ICollection<CoachEducation> Education { get; set; } = new List<CoachEducation>();
    public ICollection<CoachSpecialization> Specializations { get; set; } = new List<CoachSpecialization>();
    public ICollection<CoachDocument> Documents { get; set; } = new List<CoachDocument>();
    public ICollection<CoachAthlete> CoachAthletes { get; set; } = new List<CoachAthlete>();
}
