using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.ApproveRegistration;

public class ApproveRegistrationCommand : IRequest<Result<PlatformRegistrationDto>>
{
    public Guid RegistrationId { get; set; }
    public string? ApprovedBy { get; set; }
}
