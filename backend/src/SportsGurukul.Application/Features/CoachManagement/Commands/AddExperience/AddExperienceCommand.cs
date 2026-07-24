using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AddExperience;

public class AddExperienceCommand : IRequest<Result<ExperienceDto>>
{
    public Guid CoachId { get; set; }
    public string Organization { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Sport { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
}
