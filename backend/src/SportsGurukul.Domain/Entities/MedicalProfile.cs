using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class MedicalProfile : BaseEntity
{
    public Guid AthleteId { get; set; }
    public string? MedicalConditions { get; set; }
    public string? Allergies { get; set; }
    public string? Medications { get; set; }
    public string? BloodGroup { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? DoctorName { get; set; }
    public string? DoctorContact { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Athlete Athlete { get; set; } = null!;
}
