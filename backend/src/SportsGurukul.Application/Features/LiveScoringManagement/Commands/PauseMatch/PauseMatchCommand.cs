using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.PauseMatch;

public class PauseMatchCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
}
