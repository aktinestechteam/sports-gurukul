using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ApproveRegistration;

public class ApproveRegistrationCommand : IRequest<Result<RegistrationDto>>
{
    public Guid RegistrationId { get; set; }
}
