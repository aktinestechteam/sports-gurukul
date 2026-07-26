using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.AssignCoach;

public class AssignCoachCommand : IRequest<Result<AcademyCoachSummaryDto>>
{
    public Guid AcademyId { get; set; }
    public Guid CoachId { get; set; }
}
