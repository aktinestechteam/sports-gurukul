using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;

public class CheckInParticipantCommandHandler : IRequestHandler<CheckInParticipantCommand, Result<AttendanceDto>>
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventAttendanceService _attendanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckInParticipantCommandHandler> _logger;

    public CheckInParticipantCommandHandler(
        IEventAttendanceRepository attendanceRepository,
        IEventRepository eventRepository,
        IEventAttendanceService attendanceService,
        IUnitOfWork unitOfWork,
        ILogger<CheckInParticipantCommandHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _eventRepository = eventRepository;
        _attendanceService = attendanceService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AttendanceDto>> Handle(CheckInParticipantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking in participant {ParticipantId} for event {EventId}", request.ParticipantId, request.EventId);

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

        var canCheckIn = await _attendanceService.CanCheckInAsync(participant, cancellationToken);
        if (!canCheckIn)
        {
            _logger.LogWarning("Participant {ParticipantId} cannot check in", request.ParticipantId);
            return Result<AttendanceDto>.Failure("Participant is not eligible for check-in.");
        }

        EventAttendance? existingAttendance = null;
        if (request.SessionId.HasValue)
        {
            existingAttendance = await _attendanceRepository.GetBySessionAndParticipantAsync(request.SessionId.Value, request.ParticipantId, cancellationToken);
        }

        if (existingAttendance is not null)
        {
            existingAttendance.CheckInTime = DateTime.UtcNow;
            existingAttendance.Status = EventAttendanceStatus.CheckedIn;
            existingAttendance.UpdatedAt = DateTime.UtcNow;
            _attendanceRepository.Update(existingAttendance);
        }
        else
        {
            var attendance = new EventAttendance
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                SessionId = request.SessionId,
                ParticipantId = request.ParticipantId,
                Status = EventAttendanceStatus.CheckedIn,
                CheckInTime = DateTime.UtcNow,
                MarkedBy = "System",
                CreatedAt = DateTime.UtcNow
            };

            await _attendanceRepository.AddAsync(attendance, cancellationToken);
            existingAttendance = attendance;
        }

        participant.AttendanceStatus = EventAttendanceStatus.CheckedIn;
        participant.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Participant {ParticipantId} checked in for event {EventId}", request.ParticipantId, request.EventId);

        var sessionTitle = request.SessionId.HasValue
            ? @event.Sessions.FirstOrDefault(s => s.Id == request.SessionId.Value)?.Title
            : null;

        var dto = MapToDto(existingAttendance, participant.ParticipantName, sessionTitle);
        return Result<AttendanceDto>.Success(dto);
    }

    internal static AttendanceDto MapToDto(EventAttendance att, string participantName = "", string? sessionTitle = null)
    {
        return new AttendanceDto
        {
            Id = att.Id,
            EventId = att.EventId,
            SessionId = att.SessionId,
            SessionTitle = sessionTitle,
            ParticipantId = att.ParticipantId,
            ParticipantName = participantName,
            Status = att.Status.ToString(),
            CheckInTime = att.CheckInTime,
            CheckOutTime = att.CheckOutTime,
            Remarks = att.Remarks,
            MarkedBy = att.MarkedBy,
            CreatedAt = att.CreatedAt
        };
    }
}
