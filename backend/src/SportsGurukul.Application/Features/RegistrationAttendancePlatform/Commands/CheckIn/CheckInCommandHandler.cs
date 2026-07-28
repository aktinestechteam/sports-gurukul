using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.CheckIn;

public class CheckInCommandHandler : IRequestHandler<CheckInCommand, Result<PlatformAttendanceDto>>
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IAttendanceEngine _attendanceEngine;
    private readonly ICheckInService _checkInService;
    private readonly IQrCodeService _qrCodeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckInCommandHandler> _logger;

    public CheckInCommandHandler(
        IEventAttendanceRepository attendanceRepository,
        IEventRegistrationRepository registrationRepository,
        IAttendanceEngine attendanceEngine,
        ICheckInService checkInService,
        IQrCodeService qrCodeService,
        IUnitOfWork unitOfWork,
        ILogger<CheckInCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _registrationRepository = registrationRepository;
        _attendanceEngine = attendanceEngine;
        _checkInService = checkInService;
        _qrCodeService = qrCodeService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PlatformAttendanceDto>> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing check-in for participant {ParticipantId} on {ProgramType} {ProgramId}",
            request.ParticipantId, request.ProgramType, request.ProgramId);

        Guid? validatedParticipantId = null;
        if (!string.IsNullOrEmpty(request.QrCodeData))
        {
            var isValid = await _qrCodeService.ValidateQrCodeAsync(request.QrCodeData, QrCodeType.Attendance, cancellationToken);
            if (!isValid)
                return Result<PlatformAttendanceDto>.Failure("Invalid or expired QR code.");

            validatedParticipantId = await _checkInService.ValidateQrCodeForCheckInAsync(
                request.QrCodeData,
                async (data, ct) =>
                {
                    var isValidQr = await _qrCodeService.ValidateQrCodeAsync(data, QrCodeType.Attendance, ct);
                    return isValidQr ? request.ParticipantId : null;
                },
                cancellationToken);
        }

        var isActive = await _attendanceEngine.CanCheckInAsync(
            request.ParticipantId,
            async (pid, ct) => await _registrationRepository.IsAlreadyRegisteredAsync(request.ProgramId, pid, null, ct),
            cancellationToken);
        if (!isActive)
            return Result<PlatformAttendanceDto>.Failure("Participant is not eligible for check-in.");

        var alreadyCheckedIn = await _checkInService.IsAlreadyCheckedInAsync(
            request.ParticipantId,
            request.SessionId,
            async (pid, sid, ct) => await _attendanceRepository.GetBySessionAndParticipantAsync(sid ?? Guid.Empty, pid, ct) != null,
            cancellationToken);
        if (alreadyCheckedIn)
            return Result<PlatformAttendanceDto>.Failure("Participant is already checked in.");

        var method = _checkInService.GetCheckInMethod(!string.IsNullOrEmpty(request.QrCodeData), request.IsManual, false);

        var attendance = new EventAttendance
        {
            Id = Guid.NewGuid(),
            EventId = request.ProgramId,
            SessionId = request.SessionId,
            ParticipantId = request.ParticipantId,
            Status = EventAttendanceStatus.CheckedIn,
            CheckInTime = DateTime.UtcNow,
            Remarks = request.Remarks,
            MarkedBy = method,
            CreatedAt = DateTime.UtcNow
        };

        await _attendanceRepository.AddAsync(attendance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new PlatformAttendanceDto
        {
            Id = attendance.Id,
            ProgramType = request.ProgramType,
            ProgramId = attendance.EventId,
            SessionId = attendance.SessionId,
            ParticipantId = attendance.ParticipantId,
            Status = PlatformAttendanceStatus.Present,
            CheckInTime = attendance.CheckInTime,
            Method = method,
            Remarks = attendance.Remarks,
            CreatedAt = attendance.CreatedAt
        };

        _logger.LogInformation("Check-in completed for participant {ParticipantId} via {Method}", request.ParticipantId, method);
        return Result<PlatformAttendanceDto>.Success(dto);
    }
}
