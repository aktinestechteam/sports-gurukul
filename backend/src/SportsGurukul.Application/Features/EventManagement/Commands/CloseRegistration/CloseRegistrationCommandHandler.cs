using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CloseRegistration;

public class CloseRegistrationCommandHandler : IRequestHandler<CloseRegistrationCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventLifecycleService _eventLifecycleService;
    private readonly ILogger<CloseRegistrationCommandHandler> _logger;

    public CloseRegistrationCommandHandler(
        IEventRepository eventRepository,
        IEventLifecycleService eventLifecycleService,
        ILogger<CloseRegistrationCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventLifecycleService = eventLifecycleService;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(CloseRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Closing registration for event {EventId}", request.EventId);

        var evt = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (evt is null)
        {
            _logger.LogWarning("Event {EventId} not found", request.EventId);
            return Result<EventDto>.Failure("Event not found");
        }

        await _eventLifecycleService.ValidateStateTransitionAsync(evt.Status, EventStatus.RegistrationClosed, cancellationToken);

        evt.Status = EventStatus.RegistrationClosed;
        evt.UpdatedAt = DateTime.UtcNow;
        _eventRepository.Update(evt);

        var dto = EventDto.MapToDto(evt);

        _logger.LogInformation("Registration closed for event {EventId}", request.EventId);
        return Result<EventDto>.Success(dto);
    }
}
