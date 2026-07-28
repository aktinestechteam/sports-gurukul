using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RegisterParticipant;

public class RegisterParticipantCommandHandler : IRequestHandler<RegisterParticipantCommand, Result<RegistrationDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventRegistrationRepository _eventRegistrationRepository;
    private readonly IEventRegistrationService _eventRegistrationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterParticipantCommandHandler> _logger;

    public RegisterParticipantCommandHandler(
        IEventRepository eventRepository,
        IEventRegistrationRepository eventRegistrationRepository,
        IEventRegistrationService eventRegistrationService,
        IUnitOfWork unitOfWork,
        ILogger<RegisterParticipantCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventRegistrationRepository = eventRegistrationRepository;
        _eventRegistrationService = eventRegistrationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegistrationDto>> Handle(RegisterParticipantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering participant for event {EventId}", request.EventId);

        var evt = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (evt is null)
        {
            _logger.LogWarning("Event {EventId} not found", request.EventId);
            return Result<RegistrationDto>.Failure("Event not found");
        }

        var isAllowed = await _eventRegistrationService.IsRegistrationAllowedAsync(evt, cancellationToken);
        if (!isAllowed)
        {
            _logger.LogWarning("Registration not allowed for event {EventId}", request.EventId);
            return Result<RegistrationDto>.Failure("Registration is not allowed for this event");
        }

        var hasCapacity = await _eventRegistrationService.IsCapacityAvailableAsync(evt, cancellationToken);
        if (!hasCapacity)
        {
            _logger.LogWarning("No capacity available for event {EventId}", request.EventId);
            return Result<RegistrationDto>.Failure("Event has reached maximum capacity");
        }

        var isDuplicate = await _eventRegistrationService.IsDuplicateRegistrationAsync(request.EventId, request.AthleteId, request.UserId, cancellationToken);
        if (isDuplicate)
        {
            _logger.LogWarning("Duplicate registration attempt for event {EventId}, athlete {AthleteId}, user {UserId}", request.EventId, request.AthleteId, request.UserId);
            return Result<RegistrationDto>.Failure("Participant is already registered for this event");
        }

        var status = await _eventRegistrationService.DetermineInitialStatusAsync(evt, cancellationToken);
        var registrationNumber = await _eventRegistrationService.GenerateRegistrationNumberAsync(cancellationToken);

        var registration = new EventRegistration
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            AthleteId = request.AthleteId,
            UserId = request.UserId,
            RegistrationNumber = registrationNumber,
            Status = status,
            AmountPaid = request.AmountPaid,
            PaymentReference = request.PaymentReference,
            Notes = request.Notes,
            RegistrationDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRegistrationRepository.AddAsync(registration, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = RegistrationDtoExtensions.MapToDto(registration, evt.EventName);

        _logger.LogInformation("Participant registered for event {EventId} with registration number {RegistrationNumber}", request.EventId, registrationNumber);
        return Result<RegistrationDto>.Success(dto);
    }
}
