using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CancelEvent;

public class CancelEventCommandHandler : IRequestHandler<CancelEventCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventLifecycleService _eventLifecycleService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelEventCommandHandler> _logger;

    public CancelEventCommandHandler(
        IEventRepository eventRepository,
        IEventLifecycleService eventLifecycleService,
        IUnitOfWork unitOfWork,
        ILogger<CancelEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventLifecycleService = eventLifecycleService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(CancelEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling event: {EventId}", request.EventId);

        var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<EventDto>.Failure("Event not found.");
        }

        var canCancel = await _eventLifecycleService.CanCancelAsync(eventEntity, cancellationToken);
        if (!canCancel)
        {
            _logger.LogWarning("Cannot cancel event: {EventId}, Status: {Status}", request.EventId, eventEntity.Status);
            return Result<EventDto>.Failure("Event cannot be cancelled in its current status.");
        }

        var validatedStatus = await _eventLifecycleService.ValidateStateTransitionAsync(
            eventEntity.Status, EventStatus.Cancelled, cancellationToken);

        eventEntity.Status = validatedStatus;
        eventEntity.CancellationPolicy = request.Reason ?? eventEntity.CancellationPolicy;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _eventRepository.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event cancelled: {EventId}, Reason: {Reason}", request.EventId, request.Reason);

        var dto = CreateEventCommandHandler.MapToDto(eventEntity);
        return Result<EventDto>.Success(dto);
    }
}
