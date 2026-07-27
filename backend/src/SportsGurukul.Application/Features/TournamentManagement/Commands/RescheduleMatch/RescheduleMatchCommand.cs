using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RescheduleMatch;

public class RescheduleMatchCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
    public DateTime NewDate { get; set; }
    public TimeSpan NewTime { get; set; }
    public string? Reason { get; set; }
}
