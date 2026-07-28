using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetRegistrationStatus;

public class GetRegistrationStatusQuery : IRequest<Result<PlatformRegistrationDto>>
{
    public Guid RegistrationId { get; set; }
}
