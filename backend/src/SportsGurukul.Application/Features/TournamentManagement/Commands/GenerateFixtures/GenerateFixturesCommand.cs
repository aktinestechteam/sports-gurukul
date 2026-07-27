using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateFixtures;

public class GenerateFixturesCommand : IRequest<Result<IReadOnlyList<FixtureDto>>>
{
    public Guid TournamentId { get; set; }
}
