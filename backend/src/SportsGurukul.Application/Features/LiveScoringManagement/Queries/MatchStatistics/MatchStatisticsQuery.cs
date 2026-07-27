using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.MatchStatistics;

public class MatchStatisticsQuery : IRequest<Result<MatchStatisticsDto>>
{
    public Guid MatchId { get; set; }
}
