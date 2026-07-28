using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.SubmitFeedback;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ApproveFeedback;

public class ApproveFeedbackCommandHandler : IRequestHandler<ApproveFeedbackCommand, Result<FeedbackDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventFeedbackRepository _feedbackRepository;
    private readonly ILogger<ApproveFeedbackCommandHandler> _logger;

    public ApproveFeedbackCommandHandler(
        IEventRepository eventRepository,
        IEventFeedbackRepository feedbackRepository,
        ILogger<ApproveFeedbackCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _feedbackRepository = feedbackRepository;
        _logger = logger;
    }

    public async Task<Result<FeedbackDto>> Handle(ApproveFeedbackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving feedback: {FeedbackId}", request.FeedbackId);

        EventFeedback? feedback = null;
        string eventName = string.Empty;

        var events = await _eventRepository.GetAllAsync(cancellationToken);
        foreach (var evt in events)
        {
            var evtWithDetails = await _eventRepository.GetWithDetailsAsync(evt.Id, cancellationToken);
            if (evtWithDetails is null) continue;

            feedback = evtWithDetails.Feedbacks.FirstOrDefault(f => f.Id == request.FeedbackId);
            if (feedback is not null)
            {
                eventName = evtWithDetails.EventName;
                break;
            }
        }

        if (feedback is null)
            return Result<FeedbackDto>.Failure("Feedback not found.");

        feedback.UpdatedAt = DateTime.UtcNow;
        _feedbackRepository.Update(feedback);

        _logger.LogInformation("Feedback approved: {FeedbackId}", request.FeedbackId);

        var participantName = feedback.Participant?.ParticipantName;
        var dto = SubmitFeedbackCommandHandler.MapToDto(feedback, eventName, participantName);
        return Result<FeedbackDto>.Success(dto);
    }
}
