using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.AwardMedals;

public class AwardMedalsCommand : IRequest<Result<IReadOnlyList<AwardDto>>>
{
    public Guid TournamentId { get; set; }
}
