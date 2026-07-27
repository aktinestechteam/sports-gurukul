using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RegenerateFixtures;

public class RegenerateFixturesCommand : IRequest<Result<IReadOnlyList<FixtureDto>>>
{
    public Guid TournamentId { get; set; }
}
