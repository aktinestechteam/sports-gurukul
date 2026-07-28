using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CheckOutParticipant;

public class CheckOutParticipantCommandHandler : IRequestHandler<CheckOutParticipantCommand, Result<AttendanceDto>>
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly IEventAttendanceService _attendanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckOutParticipantCommandHandler> _logger;

    public CheckOutParticipantCommandHandler(
        IEventAttendanceRepository attendanceRepository,
        IEventAttendanceService attendanceService,
        IUnitOfWork unitOfWork,
        ILogger<CheckOutParticipantCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _attendanceService = attendanceService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AttendanceDto>> Handle(CheckOutParticipantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking out attendance {AttendanceId}", request.AttendanceId);

        var attendance = await _attendanceRepository.GetByIdAsync(request.AttendanceId, cancellationToken);
        if (attendance is null)
        {
            _logger.LogWarning("Attendance {AttendanceId} not found", request.AttendanceId);
            return Result<AttendanceDto>.Failure("Attendance record not found.");
        }

        if (attendance.CheckInTime is null)
        {
            _logger.LogWarning("Attendance {AttendanceId} has no check-in time", request.AttendanceId);
            return Result<AttendanceDto>.Failure("Cannot check out a participant who has not checked in.");
        }

        if (attendance.CheckOutTime.HasValue)
        {
            _logger.LogWarning("Attendance {AttendanceId} already has a check-out time", request.AttendanceId);
            return Result<AttendanceDto>.Failure("Participant has already checked out.");
        }

        var participant = attendance.Participant;
        var canCheckOut = await _attendanceService.CanCheckOutAsync(participant, attendance, cancellationToken);
        if (!canCheckOut)
        {
            _logger.LogWarning("Participant cannot check out for attendance {AttendanceId}", request.AttendanceId);
            return Result<AttendanceDto>.Failure("Participant is not eligible for check-out.");
        }

        attendance.CheckOutTime = DateTime.UtcNow;
        attendance.UpdatedAt = DateTime.UtcNow;
        _attendanceRepository.Update(attendance);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Participant checked out for attendance {AttendanceId}", request.AttendanceId);

        var dto = CheckInParticipantCommandHandler.MapToDto(attendance, participant.ParticipantName);
        return Result<AttendanceDto>.Success(dto);
    }
}
