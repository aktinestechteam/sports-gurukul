using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachProfile;

public class UpdateCoachProfileCommand : IRequest<Result<CoachDto>>
{
    public Guid CoachId { get; set; }
    public string? Biography { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? CurrentOrganization { get; set; }
    public string? HighestQualification { get; set; }
    public string? PreferredLanguage { get; set; }
    public CoachingLevel? CoachingLevel { get; set; }
}
