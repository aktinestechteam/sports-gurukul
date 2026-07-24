using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoach;

public class RestoreCoachCommand : IRequest<Result<Unit>>
{
    public Guid CoachId { get; set; }
}
