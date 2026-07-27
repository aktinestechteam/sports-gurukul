using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateParticipantNumbers;

public class GenerateParticipantNumbersCommand : IRequest<Result<Unit>>
{
    public Guid TournamentId { get; set; }
}
