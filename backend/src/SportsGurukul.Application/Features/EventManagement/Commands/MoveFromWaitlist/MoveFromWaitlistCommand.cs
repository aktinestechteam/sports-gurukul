using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.MoveFromWaitlist;

public class MoveFromWaitlistCommand : IRequest<Result<RegistrationDto>>
{
    public Guid RegistrationId { get; set; }
}
