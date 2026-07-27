using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.UpdateAttendance;

public class UpdateAttendanceCommandHandler : IRequestHandler<UpdateAttendanceCommand, Result<DTOs.AttendanceDto>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<UpdateAttendanceCommandHandler> _logger;

    public UpdateAttendanceCommandHandler(
        IAttendanceRepository attendanceRepository,
        ISessionRepository sessionRepository,
        ILogger<UpdateAttendanceCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.AttendanceDto>> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating attendance {AttendanceId}", request.AttendanceId);

        var attendance = await _attendanceRepository.GetByIdAsync(request.AttendanceId, cancellationToken);
        if (attendance is null)
        {
            _logger.LogWarning("Attendance {AttendanceId} not found", request.AttendanceId);
            return Result<DTOs.AttendanceDto>.Failure("Attendance record not found");
        }

        attendance.AttendanceStatus = request.Status;
        attendance.Remarks = request.Remarks;
        attendance.UpdatedAt = DateTime.UtcNow;

        _attendanceRepository.Update(attendance);

        var session = await _sessionRepository.GetByIdAsync(attendance.SessionId, cancellationToken);
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

        _logger.LogInformation("Attendance {AttendanceId} successfully updated", request.AttendanceId);
        return Result<DTOs.AttendanceDto>.Success(dto);
    }
}
