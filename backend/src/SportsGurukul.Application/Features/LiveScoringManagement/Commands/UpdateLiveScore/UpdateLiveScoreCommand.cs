using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.UpdateLiveScore;

public class UpdateLiveScoreCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public Guid ParticipantId { get; set; }
    public int Points { get; set; }
    public ScoringUnit Unit { get; set; } = ScoringUnit.Point;
    public int PeriodNumber { get; set; }
    public string? Description { get; set; }
}
