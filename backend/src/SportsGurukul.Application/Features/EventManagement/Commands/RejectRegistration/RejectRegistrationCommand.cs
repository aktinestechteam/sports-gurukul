using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RejectRegistration;

public class RejectRegistrationCommand : IRequest<Result<RegistrationDto>>
{
    public Guid RegistrationId { get; set; }
    public string? Reason { get; set; }
}
