using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetCapacityInfo;

public class GetCapacityInfoQuery : IRequest<Result<PlatformCapacityDto>>
{
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public int? MaxCapacity { get; set; }
    public bool WaitlistEnabled { get; set; }
}
