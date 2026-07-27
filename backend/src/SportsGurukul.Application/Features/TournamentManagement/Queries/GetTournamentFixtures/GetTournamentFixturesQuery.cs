using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentFixtures;

public class GetTournamentFixturesQuery : IRequest<Result<IReadOnlyList<FixtureDto>>>
{
    public Guid TournamentId { get; set; }
    public Guid? StageId { get; set; }
}
