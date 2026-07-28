using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ScheduleEvent;

public class ScheduleEventCommandHandler : IRequestHandler<ScheduleEventCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventLifecycleService _eventLifecycleService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ScheduleEventCommandHandler> _logger;

    public ScheduleEventCommandHandler(
        IEventRepository eventRepository,
        IEventLifecycleService eventLifecycleService,
        IUnitOfWork unitOfWork,
        ILogger<ScheduleEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventLifecycleService = eventLifecycleService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(ScheduleEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduling event: {EventId}", request.EventId);

        var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<EventDto>.Failure("Event not found.");
        }

        if (request.EndDate <= request.StartDate)
        {
            _logger.LogWarning("Event end date must be after start date");
            return Result<EventDto>.Failure("Event end date must be after start date.");
        }

        if (request.RegistrationCloseDate >= request.StartDate)
        {
            _logger.LogWarning("Registration close date must be before event start date");
            return Result<EventDto>.Failure("Registration close date must be before event start date.");
        }

        var validatedStatus = await _eventLifecycleService.ValidateStateTransitionAsync(
            eventEntity.Status, EventStatus.Scheduled, cancellationToken);

        eventEntity.StartDate = request.StartDate;
        eventEntity.EndDate = request.EndDate;
        eventEntity.RegistrationOpenDate = request.RegistrationOpenDate;
        eventEntity.RegistrationCloseDate = request.RegistrationCloseDate;
        eventEntity.Status = validatedStatus;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _eventRepository.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event scheduled: {EventId}", request.EventId);

        var dto = CreateEventCommandHandler.MapToDto(eventEntity);
        return Result<EventDto>.Success(dto);
    }
}
