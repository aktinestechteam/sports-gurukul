using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.MarkAttendance;

public class MarkAttendanceCommandHandler : IRequestHandler<MarkAttendanceCommand, Result<DTOs.AttendanceDto>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ITrainingBatchRepository _batchRepository;
    private readonly ILogger<MarkAttendanceCommandHandler> _logger;

    public MarkAttendanceCommandHandler(
        ISessionRepository sessionRepository,
        IAttendanceRepository attendanceRepository,
        ITrainingBatchRepository batchRepository,
        ILogger<MarkAttendanceCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
        _batchRepository = batchRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.AttendanceDto>> Handle(MarkAttendanceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking attendance for athlete {AthleteId} in session {SessionId}", request.AthleteId, request.SessionId);

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            _logger.LogWarning("Session {SessionId} not found", request.SessionId);
            return Result<DTOs.AttendanceDto>.Failure("Session not found");
        }

        var batch = await _batchRepository.GetByIdWithDetailsAsync(session.BatchId, cancellationToken);
        if (batch is null)
        {
            _logger.LogWarning("Batch {BatchId} for session {SessionId} not found", session.BatchId, request.SessionId);
            return Result<DTOs.AttendanceDto>.Failure("Associated batch not found");
        }

        var isEnrolled = batch.Enrollments?
            .Any(e => e.AthleteId == request.AthleteId && e.Status == Domain.Enums.EnrollmentStatus.Active) ?? false;
        if (!isEnrolled)
        {
            _logger.LogWarning("Athlete {AthleteId} is not enrolled in batch {BatchId}", request.AthleteId, session.BatchId);
            return Result<DTOs.AttendanceDto>.Failure("Athlete is not enrolled in the associated batch");
        }

        var existingAttendance = await _attendanceRepository.GetBySessionAndAthleteAsync(request.SessionId, request.AthleteId, cancellationToken);
        if (existingAttendance is not null)
        {
            _logger.LogWarning("Attendance already marked for athlete {AthleteId} in session {SessionId}", request.AthleteId, request.SessionId);
            return Result<DTOs.AttendanceDto>.Failure("Attendance already marked for this session");
        }

        var attendance = new SportsGurukul.Domain.Entities.Attendance
        {
            Id = Guid.NewGuid(),
            SessionId = request.SessionId,
            AthleteId = request.AthleteId,
            AttendanceStatus = request.Status,
            Remarks = request.Remarks,
            CreatedAt = DateTime.UtcNow
        };

        await _attendanceRepository.AddAsync(attendance, cancellationToken);

        var athlete = attendance.Athlete;
        var dto = new DTOs.AttendanceDto
        {
            Id = attendance.Id,
            SessionId = attendance.SessionId,
            SessionCode = session.SessionCode,
            AthleteId = attendance.AthleteId,
            AthleteName = athlete?.User?.FullName ?? string.Empty,
            AthleteCode = athlete?.AthleteCode ?? string.Empty,
            AttendanceStatus = attendance.AttendanceStatus.ToString(),
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            Remarks = attendance.Remarks,
            CreatedAt = attendance.CreatedAt
        };

        _logger.LogInformation("Attendance {AttendanceId} successfully created for athlete {AthleteId} in session {SessionId}", attendance.Id, request.AthleteId, request.SessionId);
        return Result<DTOs.AttendanceDto>.Success(dto);
    }
}
