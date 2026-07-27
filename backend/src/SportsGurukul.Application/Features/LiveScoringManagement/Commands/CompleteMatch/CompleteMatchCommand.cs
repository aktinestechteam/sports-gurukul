using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.CompleteMatch;

public class CompleteMatchCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
}
