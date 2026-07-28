using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CancelRegistration;

public class CancelRegistrationCommand : IRequest<Result<RegistrationDto>>
{
    public Guid RegistrationId { get; set; }
}
