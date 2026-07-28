using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CompleteEvent;

public class CompleteEventCommandHandler : IRequestHandler<CompleteEventCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventLifecycleService _eventLifecycleService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteEventCommandHandler> _logger;

    public CompleteEventCommandHandler(
        IEventRepository eventRepository,
        IEventLifecycleService eventLifecycleService,
        IUnitOfWork unitOfWork,
        ILogger<CompleteEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventLifecycleService = eventLifecycleService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(CompleteEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing event: {EventId}", request.EventId);

        var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<EventDto>.Failure("Event not found.");
        }

        var canComplete = await _eventLifecycleService.CanCompleteAsync(eventEntity, cancellationToken);
        if (!canComplete)
        {
            _logger.LogWarning("Cannot complete event: {EventId}, Status: {Status}", request.EventId, eventEntity.Status);
            return Result<EventDto>.Failure("Event can only be completed when it is in progress and the end date has passed.");
        }

        var validatedStatus = await _eventLifecycleService.ValidateStateTransitionAsync(
            eventEntity.Status, EventStatus.Completed, cancellationToken);

        eventEntity.Status = validatedStatus;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _eventRepository.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event completed: {EventId}", request.EventId);

        var dto = CreateEventCommandHandler.MapToDto(eventEntity);
        return Result<EventDto>.Success(dto);
    }
}
