using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Academy : BaseEntity
{
    public string AcademyCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Description { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? GSTNumber { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public string? Website { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public AcademyStatus Status { get; set; } = AcademyStatus.Pending;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public AcademyType AcademyType { get; set; } = AcademyType.MultiSport;
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public Guid? OwnedByUserId { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AcademyContact? Contact { get; set; }
    public AcademyOperatingHours? OperatingHours { get; set; }
    public AcademyVerification? Verification { get; set; }
    public ICollection<AcademyBranch> Branches { get; set; } = new List<AcademyBranch>();
    public ICollection<AcademySport> AcademySports { get; set; } = new List<AcademySport>();
    public ICollection<AcademyFacility> Facilities { get; set; } = new List<AcademyFacility>();
    public ICollection<AcademyMembership> Memberships { get; set; } = new List<AcademyMembership>();
    public ICollection<AcademyDocument> Documents { get; set; } = new List<AcademyDocument>();
    public ICollection<AcademyGallery> GalleryImages { get; set; } = new List<AcademyGallery>();
}
