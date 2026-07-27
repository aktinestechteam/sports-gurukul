using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentResults;

public class GetTournamentResultsQuery : IRequest<Result<IReadOnlyList<ResultDto>>>
{
    public Guid TournamentId { get; set; }
}
