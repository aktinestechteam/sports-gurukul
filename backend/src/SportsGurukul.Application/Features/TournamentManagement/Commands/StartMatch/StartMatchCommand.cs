using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.StartMatch;

public class StartMatchCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
}
