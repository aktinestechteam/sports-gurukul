using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.TournamentStandings;

public class TournamentStandingsQuery : IRequest<Result<StandingsDto>>
{
    public Guid TournamentId { get; set; }
    public string? SportCode { get; set; }
}
