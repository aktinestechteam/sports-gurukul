using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RejectRegistration;

public class RejectRegistrationCommand : IRequest<Result<Unit>>
{
    public Guid RegistrationId { get; set; }
    public string? Reason { get; set; }
}
