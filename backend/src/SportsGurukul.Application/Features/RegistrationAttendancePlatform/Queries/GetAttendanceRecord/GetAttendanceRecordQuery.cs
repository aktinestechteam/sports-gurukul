using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetAttendanceRecord;

public class GetAttendanceRecordQuery : IRequest<Result<PlatformAttendanceDto>>
{
    public Guid ParticipantId { get; set; }
    public Guid ProgramId { get; set; }
    public Guid? SessionId { get; set; }
}
