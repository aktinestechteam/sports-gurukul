using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RejectRegistration;

public class RejectRegistrationCommand : IRequest<Result<PlatformRegistrationDto>>
{
    public Guid RegistrationId { get; set; }
    public string? Reason { get; set; }
    public string? RejectedBy { get; set; }
}
