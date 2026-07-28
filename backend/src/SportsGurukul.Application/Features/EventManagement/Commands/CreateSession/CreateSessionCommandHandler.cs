using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CreateSession;

public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Result<EventSessionDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSessionCommandHandler> _logger;

    public CreateSessionCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSessionCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventSessionDto>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating session for event {EventId}: {Title}", request.EventId, request.Title);

        var @event = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (@event is null)
        {
            _logger.LogWarning("Event {EventId} not found", request.EventId);
            return Result<EventSessionDto>.Failure("Event not found.");
        }

        if (request.EndTime <= request.StartTime)
        {
            _logger.LogWarning("Session end time must be after start time");
            return Result<EventSessionDto>.Failure("Session end time must be after start time.");
        }

        var sessionCode = $"SES-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var session = new EventSession
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            SessionCode = sessionCode,
            Title = request.Title,
            Description = request.Description,
            SessionDate = request.SessionDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            VenueId = request.VenueId,
            SpeakerId = request.SpeakerId,
            CoachId = request.CoachId,
            Status = EventSessionStatus.Scheduled,
            Capacity = request.Capacity,
            IsBreak = request.IsBreak,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        @event.Sessions.Add(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Session created: {SessionId}, Code: {SessionCode}", session.Id, sessionCode);

        var dto = MapToDto(session);
        return Result<EventSessionDto>.Success(dto);
    }

    internal static EventSessionDto MapToDto(EventSession s)
    {
        return new EventSessionDto
        {
            Id = s.Id,
            EventId = s.EventId,
            SessionCode = s.SessionCode,
            Title = s.Title,
            Description = s.Description,
            SessionDate = s.SessionDate,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            VenueId = s.VenueId,
            SpeakerId = s.SpeakerId,
            CoachId = s.CoachId,
            Status = s.Status.ToString(),
            Capacity = s.Capacity,
            CurrentAttendeeCount = s.CurrentAttendeeCount,
            IsBreak = s.IsBreak,
            Notes = s.Notes,
            RowVersion = s.RowVersion,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}
