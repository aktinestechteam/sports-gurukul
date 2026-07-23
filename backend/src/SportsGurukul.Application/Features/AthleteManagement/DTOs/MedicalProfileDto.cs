namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class MedicalProfileDto
{
    public Guid Id { get; set; }
    public string? MedicalConditions { get; set; }
    public string? Allergies { get; set; }
    public string? Medications { get; set; }
    public string? BloodGroup { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? DoctorName { get; set; }
    public string? DoctorContact { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
