using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.GenerateLeaderboard;

public class GenerateLeaderboardCommand : IRequest<Result<Unit>>
{
    public Guid TournamentId { get; set; }
    public LeaderboardType Type { get; set; }
    public string? SportCode { get; set; }
}
