using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RecordForfeit;

public class RecordForfeitCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public Guid WinnerId { get; set; }
    public string? Notes { get; set; }
}
