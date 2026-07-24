using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachEducation;

public class GetCoachEducationQuery : IRequest<Result<IReadOnlyList<EducationDto>>>
{
    public Guid CoachId { get; set; }
}
