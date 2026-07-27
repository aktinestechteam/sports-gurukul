using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckInAthlete;

public class CheckInAthleteCommandHandler : IRequestHandler<CheckInAthleteCommand, Result<DTOs.AttendanceDto>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<CheckInAthleteCommandHandler> _logger;

    public CheckInAthleteCommandHandler(
        IAttendanceRepository attendanceRepository,
        ISessionRepository sessionRepository,
        ILogger<CheckInAthleteCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.AttendanceDto>> Handle(CheckInAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking in athlete {AthleteId} for session {SessionId}", request.AthleteId, request.SessionId);

        var attendance = await _attendanceRepository.GetBySessionAndAthleteAsync(request.SessionId, request.AthleteId, cancellationToken);
        if (attendance is null)
        {
            _logger.LogWarning("No attendance record found for athlete {AthleteId} in session {SessionId}", request.AthleteId, request.SessionId);
            return Result<DTOs.AttendanceDto>.Failure("No attendance record found. Athlete must be marked for attendance first");
        }

        if (attendance.CheckInTime.HasValue)
        {
            _logger.LogWarning("Athlete {AthleteId} has already checked in for session {SessionId}", request.AthleteId, request.SessionId);
            return Result<DTOs.AttendanceDto>.Failure("Athlete has already checked in for this session");
        }

        var now = DateTime.UtcNow;
        attendance.CheckInTime = now;
        attendance.UpdatedAt = now;

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is not null && now > session.SessionDate)
        {
            attendance.AttendanceStatus = AttendanceStatus.Late;
            _logger.LogInformation("Athlete {AthleteId} checked in late for session {SessionId}", request.AthleteId, request.SessionId);
        }
        else
        {
            attendance.AttendanceStatus = AttendanceStatus.Present;
            _logger.LogInformation("Athlete {AthleteId} checked in on time for session {SessionId}", request.AthleteId, request.SessionId);
        }

        _attendanceRepository.Update(attendance);

        var athlete = attendance.Athlete;
        var dto = new DTOs.AttendanceDto
        {
            Id = attendance.Id,
            SessionId = attendance.SessionId,
            SessionCode = session?.SessionCode ?? string.Empty,
            AthleteId = attendance.AthleteId,
            AthleteName = athlete?.User?.FullName ?? string.Empty,
            AthleteCode = athlete?.AthleteCode ?? string.Empty,
            AttendanceStatus = attendance.AttendanceStatus.ToString(),
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            Remarks = attendance.Remarks,
            CreatedAt = attendance.CreatedAt,
            UpdatedAt = attendance.UpdatedAt
        };

        _logger.LogInformation("Athlete {AthleteId} successfully checked in for session {SessionId}", request.AthleteId, request.SessionId);
        return Result<DTOs.AttendanceDto>.Success(dto);
    }
}
