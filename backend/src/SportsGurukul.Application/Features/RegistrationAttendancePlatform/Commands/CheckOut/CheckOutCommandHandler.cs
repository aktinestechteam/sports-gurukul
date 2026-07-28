using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.CheckOut;

public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, Result<PlatformAttendanceDto>>
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly ICheckOutService _checkOutService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckOutCommandHandler> _logger;

    public CheckOutCommandHandler(
        IEventAttendanceRepository attendanceRepository,
        ICheckOutService checkOutService,
        IUnitOfWork unitOfWork,
        ILogger<CheckOutCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _checkOutService = checkOutService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PlatformAttendanceDto>> Handle(CheckOutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing check-out for participant {ParticipantId} on {ProgramType} {ProgramId}",
            request.ParticipantId, request.ProgramType, request.ProgramId);

        var canCheckOut = await _checkOutService.CanCheckOutAsync(
            request.ParticipantId,
            request.SessionId,
            async (pid, sid, ct) =>
            {
                var att = await _attendanceRepository.GetBySessionAndParticipantAsync(sid ?? Guid.Empty, pid, ct);
                return att?.CheckInTime;
            },
            cancellationToken);
        if (!canCheckOut)
            return Result<PlatformAttendanceDto>.Failure("Participant is not eligible for check-out.");

        var attendance = await _attendanceRepository.GetBySessionAndParticipantAsync(
            request.SessionId ?? Guid.Empty, request.ParticipantId, cancellationToken);
        if (attendance is null)
            return Result<PlatformAttendanceDto>.Failure("Attendance record not found.");

        attendance.CheckOutTime = DateTime.UtcNow;
        attendance.Status = EventAttendanceStatus.Present;
        attendance.Remarks = request.Remarks;
        attendance.UpdatedAt = DateTime.UtcNow;

        _attendanceRepository.Update(attendance);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var duration = await _checkOutService.CalculateDurationAsync(attendance.CheckInTime!.Value, attendance.CheckOutTime.Value);

        var dto = new PlatformAttendanceDto
        {
            Id = attendance.Id,
            ProgramType = request.ProgramType,
            ProgramId = attendance.EventId,
            SessionId = attendance.SessionId,
            ParticipantId = attendance.ParticipantId,
            Status = PlatformAttendanceStatus.Present,
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            Method = attendance.MarkedBy,
            Remarks = attendance.Remarks,
            CreatedAt = attendance.CreatedAt
        };

        _logger.LogInformation("Check-out completed for participant {ParticipantId}. Duration: {Duration}", request.ParticipantId, duration);
        return Result<PlatformAttendanceDto>.Success(dto);
    }
}
