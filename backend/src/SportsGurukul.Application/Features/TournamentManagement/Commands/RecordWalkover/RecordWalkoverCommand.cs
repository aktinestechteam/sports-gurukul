using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RecordWalkover;

public class RecordWalkoverCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public Guid WinnerId { get; set; }
    public string? Notes { get; set; }
}
