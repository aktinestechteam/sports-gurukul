using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetWaitlistPosition;

public class GetWaitlistPositionQuery : IRequest<Result<PlatformWaitlistDto>>
{
    public Guid ProgramId { get; set; }
    public Guid ParticipantId { get; set; }
}
