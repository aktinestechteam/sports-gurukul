using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.UpcomingEvents;

public class UpcomingEventsQueryHandler : IRequestHandler<UpcomingEventsQuery, Result<IReadOnlyList<EventCardDto>>>
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ILogger<UpcomingEventsQueryHandler> _logger;

    public UpcomingEventsQueryHandler(
        IEventSearchRepository searchRepository,
        ILogger<UpcomingEventsQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<EventCardDto>>> Handle(UpcomingEventsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting upcoming events: City={City}, Limit={Limit}", request.City, request.Limit);

        var events = await _searchRepository.GetUpcomingEventsAsync(request.Limit, DateTime.UtcNow, cancellationToken);

        var items = events.Select(e =>
        {
            var daysUntilStart = (e.StartDate - DateTime.UtcNow).Days;
            return new EventCardDto
            {
                Id = e.Id,
                EventCode = e.EventCode,
                EventName = e.EventName,
                ShortDescription = e.ShortDescription,
                BannerUrl = e.BannerUrl,
                EventType = e.EventType?.Name,
                Status = e.Status.ToString(),
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                RegistrationCloseDate = e.RegistrationCloseDate,
                MaxParticipants = e.MaxParticipants,
                RegistrationFee = e.RegistrationFee,
                IsFeatured = e.IsFeatured,
                DaysUntilStart = daysUntilStart,
                IsRegistrationOpen = e.Status == Domain.Enums.EventStatus.RegistrationOpen
            };
        }).ToList();

        return Result<IReadOnlyList<EventCardDto>>.Success(items);
    }
}
