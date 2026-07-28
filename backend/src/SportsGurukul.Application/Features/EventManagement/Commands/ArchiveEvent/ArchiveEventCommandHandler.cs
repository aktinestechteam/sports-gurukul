using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ArchiveEvent;

public class ArchiveEventCommandHandler : IRequestHandler<ArchiveEventCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventLifecycleService _eventLifecycleService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ArchiveEventCommandHandler> _logger;

    public ArchiveEventCommandHandler(
        IEventRepository eventRepository,
        IEventLifecycleService eventLifecycleService,
        IUnitOfWork unitOfWork,
        ILogger<ArchiveEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventLifecycleService = eventLifecycleService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(ArchiveEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving event: {EventId}", request.EventId);

        var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<EventDto>.Failure("Event not found.");
        }

        var canArchive = await _eventLifecycleService.CanArchiveAsync(eventEntity, cancellationToken);
        if (!canArchive)
        {
            _logger.LogWarning("Cannot archive event: {EventId}, Status: {Status}", request.EventId, eventEntity.Status);
            return Result<EventDto>.Failure("Event can only be archived when it is completed or cancelled.");
        }

        var validatedStatus = await _eventLifecycleService.ValidateStateTransitionAsync(
            eventEntity.Status, EventStatus.Archived, cancellationToken);

        eventEntity.Status = validatedStatus;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _eventRepository.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event archived: {EventId}", request.EventId);

        var dto = CreateEventCommandHandler.MapToDto(eventEntity);
        return Result<EventDto>.Success(dto);
    }
}
