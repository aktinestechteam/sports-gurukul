using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.MedalTable;

public class MedalTableQuery : IRequest<Result<MedalTableDto>>
{
    public Guid TournamentId { get; set; }
}
