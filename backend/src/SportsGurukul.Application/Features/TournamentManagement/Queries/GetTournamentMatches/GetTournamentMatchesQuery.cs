using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentMatches;

public class GetTournamentMatchesQuery : IRequest<Result<IReadOnlyList<MatchDto>>>
{
    public Guid TournamentId { get; set; }
    public MatchStatus? Status { get; set; }
    public Guid? RoundId { get; set; }
}
