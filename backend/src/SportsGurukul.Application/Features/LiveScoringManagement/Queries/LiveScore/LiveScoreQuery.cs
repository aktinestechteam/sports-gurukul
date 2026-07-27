using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.LiveScore;

public class LiveScoreQuery : IRequest<Result<LiveScoreDto>>
{
    public Guid MatchId { get; set; }
}
