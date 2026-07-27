using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.Leaderboard;

public class LeaderboardQuery : IRequest<Result<LeaderboardDto>>
{
    public Guid TournamentId { get; set; }
    public LeaderboardType Type { get; set; }
    public string? SportCode { get; set; }
}
