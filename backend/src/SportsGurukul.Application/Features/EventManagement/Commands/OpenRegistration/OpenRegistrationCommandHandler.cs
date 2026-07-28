using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.OpenRegistration;

public class OpenRegistrationCommandHandler : IRequestHandler<OpenRegistrationCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventLifecycleService _eventLifecycleService;
    private readonly ILogger<OpenRegistrationCommandHandler> _logger;

    public OpenRegistrationCommandHandler(
        IEventRepository eventRepository,
        IEventLifecycleService eventLifecycleService,
        ILogger<OpenRegistrationCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventLifecycleService = eventLifecycleService;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(OpenRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Opening registration for event {EventId}", request.EventId);

        var evt = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (evt is null)
        {
            _logger.LogWarning("Event {EventId} not found", request.EventId);
            return Result<EventDto>.Failure("Event not found");
        }

        await _eventLifecycleService.ValidateStateTransitionAsync(evt.Status, EventStatus.RegistrationOpen, cancellationToken);

        evt.Status = EventStatus.RegistrationOpen;
        evt.UpdatedAt = DateTime.UtcNow;
        _eventRepository.Update(evt);

        var dto = EventDto.MapToDto(evt);

        _logger.LogInformation("Registration opened for event {EventId}", request.EventId);
        return Result<EventDto>.Success(dto);
    }
}
