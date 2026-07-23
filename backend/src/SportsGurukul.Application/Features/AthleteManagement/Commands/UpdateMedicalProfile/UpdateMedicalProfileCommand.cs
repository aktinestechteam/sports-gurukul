using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateMedicalProfile;

public class UpdateMedicalProfileCommand : IRequest<Result<MedicalProfileDto>>
{
    public Guid AthleteId { get; set; }
    public string? MedicalConditions { get; set; }
    public string? Allergies { get; set; }
    public string? Medications { get; set; }
    public string? BloodGroup { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? DoctorName { get; set; }
    public string? DoctorContact { get; set; }
}
