using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckOutAthlete;

public class CheckOutAthleteCommandHandler : IRequestHandler<CheckOutAthleteCommand, Result<DTOs.AttendanceDto>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<CheckOutAthleteCommandHandler> _logger;

    public CheckOutAthleteCommandHandler(
        IAttendanceRepository attendanceRepository,
        ISessionRepository sessionRepository,
        ILogger<CheckOutAthleteCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.AttendanceDto>> Handle(CheckOutAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking out athlete {AthleteId} from session {SessionId}", request.AthleteId, request.SessionId);

        var attendance = await _attendanceRepository.GetBySessionAndAthleteAsync(request.SessionId, request.AthleteId, cancellationToken);
        if (attendance is null)
        {
            _logger.LogWarning("No attendance record found for athlete {AthleteId} in session {SessionId}", request.AthleteId, request.SessionId);
            return Result<DTOs.AttendanceDto>.Failure("No attendance record found for this session");
        }

        if (!attendance.CheckInTime.HasValue)
        {
            _logger.LogWarning("Athlete {AthleteId} has not checked in for session {SessionId} yet", request.AthleteId, request.SessionId);
            return Result<DTOs.AttendanceDto>.Failure("Athlete has not checked in yet");
        }

        if (attendance.CheckOutTime.HasValue)
        {
            _logger.LogWarning("Athlete {AthleteId} has already checked out from session {SessionId}", request.AthleteId, request.SessionId);
            return Result<DTOs.AttendanceDto>.Failure("Athlete has already checked out from this session");
        }

        var now = DateTime.UtcNow;
        attendance.CheckOutTime = now;
        attendance.UpdatedAt = now;

        _attendanceRepository.Update(attendance);

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
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

        _logger.LogInformation("Athlete {AthleteId} successfully checked out from session {SessionId}", request.AthleteId, request.SessionId);
        return Result<DTOs.AttendanceDto>.Success(dto);
    }
}
