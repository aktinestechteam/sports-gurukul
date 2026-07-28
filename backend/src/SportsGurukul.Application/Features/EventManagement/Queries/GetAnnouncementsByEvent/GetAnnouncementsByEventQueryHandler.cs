using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetAnnouncementsByEvent;

public class GetAnnouncementsByEventQueryHandler : IRequestHandler<GetAnnouncementsByEventQuery, Result<List<AnnouncementDto>>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetAnnouncementsByEventQueryHandler> _logger;

    public GetAnnouncementsByEventQueryHandler(
        IEventRepository eventRepository,
        ILogger<GetAnnouncementsByEventQueryHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<Result<List<AnnouncementDto>>> Handle(GetAnnouncementsByEventQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting announcements for event: {EventId}", request.EventId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<List<AnnouncementDto>>.Failure("Event not found.");
        }

        var announcements = (evt.Announcements?.ToList() ?? [])
            .Where(a => a.IsPublished && !a.IsDeleted)
            .Select(a => new AnnouncementDto
            {
                Id = a.Id,
                EventId = a.EventId,
                EventName = evt.EventName,
                Title = a.Title,
                Message = a.Message,
                IsPublished = a.IsPublished,
                PublishedAt = a.PublishedAt,
                SendNotification = a.SendNotification,
                Priority = a.Priority,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList();

        return Result<List<AnnouncementDto>>.Success(announcements);
    }
}
