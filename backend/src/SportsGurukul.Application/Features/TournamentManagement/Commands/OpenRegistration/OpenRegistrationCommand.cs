using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.OpenRegistration;

public class OpenRegistrationCommand : IRequest<Result<TournamentDto>>
{
    public Guid TournamentId { get; set; }
    public DateTime? RegistrationCloseDate { get; set; }
}
