using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.CloseRegistration;

public class CloseRegistrationCommand : IRequest<Result<TournamentDto>>
{
    public Guid TournamentId { get; set; }
}
