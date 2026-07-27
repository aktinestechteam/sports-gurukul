using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.WithdrawParticipant;

public class WithdrawParticipantCommand : IRequest<Result<Unit>>
{
    public Guid TournamentId { get; set; }
    public Guid ParticipantId { get; set; }
    public string? Reason { get; set; }
}
