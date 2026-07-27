using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentRankings;

public class GetTournamentRankingsQuery : IRequest<Result<IReadOnlyList<RankingDto>>>
{
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
}
