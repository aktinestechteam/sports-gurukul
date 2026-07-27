using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentById;

public class GetTournamentByIdQuery : IRequest<Result<TournamentDto>>
{
    public Guid TournamentId { get; set; }
}
