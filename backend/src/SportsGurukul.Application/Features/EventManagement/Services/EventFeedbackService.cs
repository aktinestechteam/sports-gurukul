using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public class EventFeedbackService : IEventFeedbackService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventFeedbackRepository _feedbackRepository;
    private readonly ILogger<EventFeedbackService> _logger;

    public EventFeedbackService(
        IEventRepository eventRepository,
        IEventFeedbackRepository feedbackRepository,
        ILogger<EventFeedbackService> logger)
    {
        _eventRepository = eventRepository;
        _feedbackRepository = feedbackRepository;
        _logger = logger;
    }

    public async Task<bool> CanSubmitFeedbackAsync(Event evt, Guid userId, CancellationToken cancellationToken = default)
    {
        if (evt.Status != EventStatus.Completed) return false;
        var existing = await _feedbackRepository.GetByEventAndUserAsync(evt.Id, userId, cancellationToken);
        return existing == null;
    }

    public async Task<double> CalculateAverageRatingAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _feedbackRepository.GetAverageRatingAsync(eventId, cancellationToken);
    }
}
