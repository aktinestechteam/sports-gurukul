using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetFeedbackByEvent;

public class GetFeedbackByEventQueryHandler : IRequestHandler<GetFeedbackByEventQuery, Result<List<FeedbackDto>>>
{
    private readonly IEventFeedbackRepository _feedbackRepository;
    private readonly ILogger<GetFeedbackByEventQueryHandler> _logger;

    public GetFeedbackByEventQueryHandler(
        IEventFeedbackRepository feedbackRepository,
        ILogger<GetFeedbackByEventQueryHandler> logger)
    {
        _feedbackRepository = feedbackRepository;
        _logger = logger;
    }

    public async Task<Result<List<FeedbackDto>>> Handle(GetFeedbackByEventQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting feedback for event: {EventId}", request.EventId);

        var feedbacks = await _feedbackRepository.GetByEventIdAsync(request.EventId, cancellationToken);

        var items = feedbacks.Select(f => new FeedbackDto
        {
            Id = f.Id,
            EventId = f.EventId,
            ParticipantId = f.ParticipantId,
            UserId = f.UserId,
            OverallRating = f.OverallRating.ToString(),
            ContentRating = f.ContentRating?.ToString(),
            SpeakerRating = f.SpeakerRating?.ToString(),
            VenueRating = f.VenueRating?.ToString(),
            OrganizationRating = f.OrganizationRating?.ToString(),
            Comments = f.Comments,
            Suggestions = f.Suggestions,
            WouldRecommend = f.WouldRecommend,
            IsAnonymous = f.IsAnonymous,
            CreatedAt = f.CreatedAt
        }).ToList();

        return Result<List<FeedbackDto>>.Success(items);
    }
}
