using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.AssignCourt;

public class AssignCourtCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public Guid TournamentCourtId { get; set; }
}
