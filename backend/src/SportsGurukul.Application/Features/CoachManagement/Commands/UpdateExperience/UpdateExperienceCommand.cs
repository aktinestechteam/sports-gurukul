using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateExperience;

public class UpdateExperienceCommand : IRequest<Result<ExperienceDto>>
{
    public Guid ExperienceId { get; set; }
    public string? Organization { get; set; }
    public string? Role { get; set; }
    public string? Sport { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
}
