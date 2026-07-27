using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.CancelTournament;

public class CancelTournamentCommand : IRequest<Result<Unit>>
{
    public Guid TournamentId { get; set; }
    public string? Reason { get; set; }
}
