using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.MarkAttendance;

public class MarkAttendanceCommandHandler : IRequestHandler<MarkAttendanceCommand, Result<AttendanceDto>>
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkAttendanceCommandHandler> _logger;

    public MarkAttendanceCommandHandler(
        IEventAttendanceRepository attendanceRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<MarkAttendanceCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AttendanceDto>> Handle(MarkAttendanceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking attendance for participant {ParticipantId} in event {EventId} as {Status}", request.ParticipantId, request.EventId, request.Status);

        var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (@event is null)
        {
            _logger.LogWarning("Event {EventId} not found", request.EventId);
            return Result<AttendanceDto>.Failure("Event not found.");
        }

        var participant = @event.Participants.FirstOrDefault(p => p.Id == request.ParticipantId);
        if (participant is null)
        {
            _logger.LogWarning("Participant {ParticipantId} not found in event {EventId}", request.ParticipantId, request.EventId);
            return Result<AttendanceDto>.Failure("Participant not found in this event.");
        }

        EventAttendance? attendance = null;
        if (request.SessionId.HasValue)
        {
            attendance = await _attendanceRepository.GetBySessionAndParticipantAsync(request.SessionId.Value, request.ParticipantId, cancellationToken);
        }

        if (attendance is not null)
        {
            attendance.Status = request.Status;
            attendance.Remarks = request.Remarks;
            attendance.MarkedBy = "System";
            attendance.UpdatedAt = DateTime.UtcNow;
            _attendanceRepository.Update(attendance);
        }
        else
        {
            attendance = new EventAttendance
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                SessionId = request.SessionId,
                ParticipantId = request.ParticipantId,
                Status = request.Status,
                Remarks = request.Remarks,
                MarkedBy = "System",
                CreatedAt = DateTime.UtcNow
            };
            await _attendanceRepository.AddAsync(attendance, cancellationToken);
        }

        participant.AttendanceStatus = request.Status;
        participant.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Attendance marked for participant {ParticipantId} as {Status}", request.ParticipantId, request.Status);

        var sessionTitle = request.SessionId.HasValue
            ? @event.Sessions.FirstOrDefault(s => s.Id == request.SessionId.Value)?.Title
            : null;

        var dto = CheckInParticipantCommandHandler.MapToDto(attendance, participant.ParticipantName, sessionTitle);
        return Result<AttendanceDto>.Success(dto);
    }
}
