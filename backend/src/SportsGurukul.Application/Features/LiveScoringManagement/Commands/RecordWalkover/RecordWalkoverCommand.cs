using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.RecordWalkover;

public class RecordWalkoverCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public Guid WinnerId { get; set; }
    public string WinnerName { get; set; } = string.Empty;
}
