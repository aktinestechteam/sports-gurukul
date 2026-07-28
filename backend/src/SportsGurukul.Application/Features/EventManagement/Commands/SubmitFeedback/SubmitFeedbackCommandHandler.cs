using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.SubmitFeedback;

public class SubmitFeedbackCommandHandler : IRequestHandler<SubmitFeedbackCommand, Result<FeedbackDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventFeedbackService _feedbackService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitFeedbackCommandHandler> _logger;

    public SubmitFeedbackCommandHandler(
        IEventRepository eventRepository,
        IEventFeedbackService feedbackService,
        IUnitOfWork unitOfWork,
        ILogger<SubmitFeedbackCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _feedbackService = feedbackService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<FeedbackDto>> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submitting feedback for event: {EventId} by user: {UserId}", request.EventId, request.UserId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
            return Result<FeedbackDto>.Failure("Event not found.");

        var canSubmit = await _feedbackService.CanSubmitFeedbackAsync(evt, request.UserId, cancellationToken);
        if (!canSubmit)
            return Result<FeedbackDto>.Failure("Feedback cannot be submitted for this event.");

        var participant = evt.Participants.FirstOrDefault(p => p.UserId == request.UserId);

        var feedback = new EventFeedback
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            ParticipantId = participant?.Id,
            UserId = request.UserId,
            OverallRating = (EventFeedbackRating)request.OverallRating,
            ContentRating = request.ContentRating.HasValue ? (EventFeedbackRating)request.ContentRating.Value : null,
            SpeakerRating = request.SpeakerRating.HasValue ? (EventFeedbackRating)request.SpeakerRating.Value : null,
            VenueRating = request.VenueRating.HasValue ? (EventFeedbackRating)request.VenueRating.Value : null,
            OrganizationRating = request.OrganizationRating.HasValue ? (EventFeedbackRating)request.OrganizationRating.Value : null,
            Comments = request.Comments,
            Suggestions = request.Suggestions,
            WouldRecommend = request.WouldRecommend,
            IsAnonymous = request.IsAnonymous,
            CreatedAt = DateTime.UtcNow
        };

        evt.Feedbacks.Add(feedback);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Feedback submitted: {FeedbackId}", feedback.Id);

        var dto = MapToDto(feedback, evt.EventName, participant?.ParticipantName);
        return Result<FeedbackDto>.Success(dto);
    }

    internal static FeedbackDto MapToDto(EventFeedback fb, string eventName = "", string? participantName = null)
    {
        return new FeedbackDto
        {
            Id = fb.Id,
            EventId = fb.EventId,
            EventName = eventName,
            ParticipantId = fb.ParticipantId,
            ParticipantName = participantName,
            UserId = fb.UserId,
            OverallRating = fb.OverallRating.ToString(),
            ContentRating = fb.ContentRating?.ToString(),
            SpeakerRating = fb.SpeakerRating?.ToString(),
            VenueRating = fb.VenueRating?.ToString(),
            OrganizationRating = fb.OrganizationRating?.ToString(),
            Comments = fb.Comments,
            Suggestions = fb.Suggestions,
            WouldRecommend = fb.WouldRecommend,
            IsAnonymous = fb.IsAnonymous,
            CreatedAt = fb.CreatedAt
        };
    }
}
