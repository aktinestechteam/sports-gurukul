using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.PublishTournament;

public class PublishTournamentCommand : IRequest<Result<TournamentDto>>
{
    public Guid TournamentId { get; set; }
}
