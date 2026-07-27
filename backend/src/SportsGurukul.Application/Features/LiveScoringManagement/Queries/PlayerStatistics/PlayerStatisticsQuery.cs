using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.PlayerStatistics;

public class PlayerStatisticsQuery : IRequest<Result<PlayerStatisticsDto>>
{
    public Guid TournamentId { get; set; }
    public Guid PlayerId { get; set; }
    public string? SportCode { get; set; }
}
