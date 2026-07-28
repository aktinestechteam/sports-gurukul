using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetAttendanceRecord;

public class GetAttendanceRecordQueryHandler : IRequestHandler<GetAttendanceRecordQuery, Result<PlatformAttendanceDto>>
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly ILogger<GetAttendanceRecordQueryHandler> _logger;

    public GetAttendanceRecordQueryHandler(
        IEventAttendanceRepository attendanceRepository,
        ILogger<GetAttendanceRecordQueryHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _logger = logger;
    }

    public async Task<Result<PlatformAttendanceDto>> Handle(GetAttendanceRecordQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching attendance for participant {ParticipantId} on program {ProgramId}", request.ParticipantId, request.ProgramId);

        var attendance = await _attendanceRepository.GetBySessionAndParticipantAsync(
            request.SessionId ?? Guid.Empty, request.ParticipantId, cancellationToken);

        if (attendance is null)
            return Result<PlatformAttendanceDto>.Failure("Attendance record not found.");

        var status = attendance.Status switch
        {
            EventAttendanceStatus.Present => PlatformAttendanceStatus.Present,
            EventAttendanceStatus.Absent => PlatformAttendanceStatus.Absent,
            EventAttendanceStatus.Late => PlatformAttendanceStatus.Late,
            EventAttendanceStatus.CheckedIn => PlatformAttendanceStatus.Present,
            _ => PlatformAttendanceStatus.Absent
        };

        var dto = new PlatformAttendanceDto
        {
            Id = attendance.Id,
            ProgramId = attendance.EventId,
            SessionId = attendance.SessionId,
            ParticipantId = attendance.ParticipantId,
            Status = status,
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            Method = attendance.MarkedBy,
            Remarks = attendance.Remarks,
            CreatedAt = attendance.CreatedAt
        };

        _logger.LogInformation("Attendance record found: {AttendanceId}, status: {Status}", attendance.Id, status);
        return Result<PlatformAttendanceDto>.Success(dto);
    }
}
