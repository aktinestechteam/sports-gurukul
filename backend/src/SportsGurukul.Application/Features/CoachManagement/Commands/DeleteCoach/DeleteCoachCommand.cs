using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoach;

public class DeleteCoachCommand : IRequest<Result<Unit>>
{
    public Guid CoachId { get; set; }
}
