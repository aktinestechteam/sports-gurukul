using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.UndoScore;

public class UndoScoreCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
}
