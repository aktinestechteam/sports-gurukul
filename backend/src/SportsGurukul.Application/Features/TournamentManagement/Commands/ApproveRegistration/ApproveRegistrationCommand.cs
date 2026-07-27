using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.ApproveRegistration;

public class ApproveRegistrationCommand : IRequest<Result<Unit>>
{
    public Guid RegistrationId { get; set; }
}
