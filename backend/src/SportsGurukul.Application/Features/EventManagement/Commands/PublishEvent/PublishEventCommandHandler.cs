using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.PublishEvent;

public class PublishEventCommandHandler : IRequestHandler<PublishEventCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventLifecycleService _eventLifecycleService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishEventCommandHandler> _logger;

    public PublishEventCommandHandler(
        IEventRepository eventRepository,
        IEventLifecycleService eventLifecycleService,
        IUnitOfWork unitOfWork,
        ILogger<PublishEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventLifecycleService = eventLifecycleService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(PublishEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing event: {EventId}", request.EventId);

        var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<EventDto>.Failure("Event not found.");
        }

        var canPublish = await _eventLifecycleService.CanPublishAsync(eventEntity, cancellationToken);
        if (!canPublish)
        {
            _logger.LogWarning("Cannot publish event: {EventId}", request.EventId);
            return Result<EventDto>.Failure("Event cannot be published. Ensure the event has a name, start date is in the future, and end date is after start date.");
        }

        var validatedStatus = await _eventLifecycleService.ValidateStateTransitionAsync(
            eventEntity.Status, EventStatus.Published, cancellationToken);

        eventEntity.Status = validatedStatus;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _eventRepository.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event published: {EventId}", request.EventId);

        var dto = CreateEventCommandHandler.MapToDto(eventEntity);
        return Result<EventDto>.Success(dto);
    }
}
