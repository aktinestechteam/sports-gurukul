using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveCoach;

public class RemoveCoachCommand : IRequest<Result<Unit>>
{
    public Guid AcademyId { get; set; }
    public Guid CoachId { get; set; }
}
