using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetSessionsByEvent;

public class GetSessionsByEventQueryHandler : IRequestHandler<GetSessionsByEventQuery, Result<List<EventSessionDto>>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetSessionsByEventQueryHandler> _logger;

    public GetSessionsByEventQueryHandler(
        IEventRepository eventRepository,
        ILogger<GetSessionsByEventQueryHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<Result<List<EventSessionDto>>> Handle(GetSessionsByEventQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting sessions for event: {EventId}", request.EventId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<List<EventSessionDto>>.Failure("Event not found.");
        }

        var sessions = (evt.Sessions?.ToList() ?? []).Select(s => new EventSessionDto
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
            VenueName = s.Venue?.VenueName,
            SpeakerId = s.SpeakerId,
            SpeakerName = s.Speaker?.SpeakerName,
            CoachId = s.CoachId,
            CoachName = s.Coach?.Coach?.User?.FullName,
            Status = s.Status.ToString(),
            Capacity = s.Capacity,
            CurrentAttendeeCount = s.CurrentAttendeeCount,
            IsBreak = s.IsBreak,
            Notes = s.Notes,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).ToList();

        return Result<List<EventSessionDto>>.Success(sessions);
    }
}
