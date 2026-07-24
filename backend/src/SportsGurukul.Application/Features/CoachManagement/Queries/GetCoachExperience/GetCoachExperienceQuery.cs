using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachExperience;

public class GetCoachExperienceQuery : IRequest<Result<IReadOnlyList<ExperienceDto>>>
{
    public Guid CoachId { get; set; }
}
