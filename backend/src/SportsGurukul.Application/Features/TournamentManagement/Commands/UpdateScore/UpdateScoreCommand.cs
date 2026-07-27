using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateScore;

public class UpdateScoreCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string? ScoreDetails { get; set; }
}
