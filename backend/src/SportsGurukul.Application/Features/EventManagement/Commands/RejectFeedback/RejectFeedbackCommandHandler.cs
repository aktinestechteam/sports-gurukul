using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.SubmitFeedback;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RejectFeedback;

public class RejectFeedbackCommandHandler : IRequestHandler<RejectFeedbackCommand, Result<FeedbackDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventFeedbackRepository _feedbackRepository;
    private readonly ILogger<RejectFeedbackCommandHandler> _logger;

    public RejectFeedbackCommandHandler(
        IEventRepository eventRepository,
        IEventFeedbackRepository feedbackRepository,
        ILogger<RejectFeedbackCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _feedbackRepository = feedbackRepository;
        _logger = logger;
    }

    public async Task<Result<FeedbackDto>> Handle(RejectFeedbackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting feedback: {FeedbackId}", request.FeedbackId);

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

        _logger.LogInformation("Feedback rejected: {FeedbackId}, Reason: {Reason}", request.FeedbackId, request.Reason);

        var participantName = feedback.Participant?.ParticipantName;
        var dto = SubmitFeedbackCommandHandler.MapToDto(feedback, eventName, participantName);
        return Result<FeedbackDto>.Success(dto);
    }
}
