using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.CalendarEvents;

public class CalendarEventsQueryHandler : IRequestHandler<CalendarEventsQuery, Result<IReadOnlyList<CalendarEventDto>>>
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ILogger<CalendarEventsQueryHandler> _logger;

    public CalendarEventsQueryHandler(
        IEventSearchRepository searchRepository,
        ILogger<CalendarEventsQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<CalendarEventDto>>> Handle(CalendarEventsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting calendar events: {FromDate} to {ToDate}, View={View}",
            request.FromDate, request.ToDate, request.ViewType);

        var events = await _searchRepository.GetCalendarEventsAsync(
            request.FromDate, request.ToDate, request.AcademyId, cancellationToken);

        var items = events.Select(e =>
        {
            var primaryVenue = e.Venues?.FirstOrDefault(v => v.IsPrimary);
            var schedule = e.Schedules?.FirstOrDefault(s => s.ScheduleDate >= request.FromDate && s.ScheduleDate <= request.ToDate);
            var color = e.Status switch
            {
                EventStatus.Published => "#3B82F6",
                EventStatus.RegistrationOpen => "#10B981",
                EventStatus.InProgress => "#F59E0B",
                EventStatus.Completed => "#6B7280",
                EventStatus.Cancelled => "#EF4444",
                _ => "#3B82F6"
            };

            return new CalendarEventDto
            {
                Id = e.Id,
                EventCode = e.EventCode,
                EventName = e.EventName,
                Description = e.ShortDescription,
                BannerUrl = e.BannerUrl,
                EventType = e.EventType?.Name ?? string.Empty,
                Status = e.Status.ToString(),
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                StartTime = schedule?.StartTime ?? TimeSpan.Zero,
                EndTime = schedule?.EndTime ?? TimeSpan.FromHours(1),
                IsAllDay = schedule?.IsAllDay ?? true,
                VenueName = primaryVenue?.VenueName,
                City = primaryVenue?.City,
                AcademyName = e.Academy?.Name ?? string.Empty,
                Color = color,
                RegistrationCount = e.Registrations?.Count(r => r.Status == EventRegistrationStatus.Approved) ?? 0,
                MaxParticipants = e.MaxParticipants,
                IsRegistrationOpen = e.Status == EventStatus.RegistrationOpen
            };
        }).ToList();

        return Result<IReadOnlyList<CalendarEventDto>>.Success(items);
    }
}
