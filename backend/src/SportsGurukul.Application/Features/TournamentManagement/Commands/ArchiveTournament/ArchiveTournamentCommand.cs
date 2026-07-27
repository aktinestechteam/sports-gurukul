using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.ArchiveTournament;

public class ArchiveTournamentCommand : IRequest<Result<Unit>>
{
    public Guid TournamentId { get; set; }
}
