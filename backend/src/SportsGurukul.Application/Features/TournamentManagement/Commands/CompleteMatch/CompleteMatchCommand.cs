using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.CompleteMatch;

public class CompleteMatchCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public Guid? WinnerId { get; set; }
}
