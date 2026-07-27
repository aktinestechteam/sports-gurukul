using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.StartMatch;

public class StartLiveMatchCommand : IRequest<Result<Guid>>
{
    public Guid TournamentId { get; set; }
    public Guid MatchId { get; set; }
    public string SportCode { get; set; } = string.Empty;
    public Guid HomeParticipantId { get; set; }
    public string HomeParticipantName { get; set; } = string.Empty;
    public Guid AwayParticipantId { get; set; }
    public string AwayParticipantName { get; set; } = string.Empty;
}
