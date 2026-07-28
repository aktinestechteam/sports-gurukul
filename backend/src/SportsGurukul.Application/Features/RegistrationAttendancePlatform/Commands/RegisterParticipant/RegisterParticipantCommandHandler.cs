using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RegisterParticipant;

public class RegisterParticipantCommandHandler : IRequestHandler<RegisterParticipantCommand, Result<PlatformRegistrationDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IRegistrationEngine _registrationEngine;
    private readonly ICapacityManagementService _capacityManagementService;
    private readonly IWaitlistEngine _waitlistEngine;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterParticipantCommandHandler> _logger;

    public RegisterParticipantCommandHandler(
        IEventRepository eventRepository,
        IEventRegistrationRepository registrationRepository,
        IRegistrationEngine registrationEngine,
        ICapacityManagementService capacityManagementService,
        IWaitlistEngine waitlistEngine,
        IUnitOfWork unitOfWork,
        ILogger<RegisterParticipantCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _registrationRepository = registrationRepository;
        _registrationEngine = registrationEngine;
        _capacityManagementService = capacityManagementService;
        _waitlistEngine = waitlistEngine;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PlatformRegistrationDto>> Handle(RegisterParticipantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing registration for {ProgramType} {ProgramId}", request.ProgramType, request.ProgramId);

        var isEligible = await _registrationEngine.ValidateRegistrationEligibilityAsync(
            request.ProgramType, request.ProgramId, request.AthleteId, request.UserId, cancellationToken);
        if (!isEligible)
            return Result<PlatformRegistrationDto>.Failure("Participant is not eligible for registration.");

        var isDuplicate = await _registrationEngine.IsDuplicateRegistrationAsync(
            request.ProgramType, request.ProgramId, request.AthleteId, request.UserId,
            async (pt, pid, aid, uid, ct) => await _registrationRepository.IsAlreadyRegisteredAsync(pid, aid, uid, ct),
            cancellationToken);
        if (isDuplicate)
            return Result<PlatformRegistrationDto>.Failure("Participant is already registered for this program.");

        var currentCount = await _registrationRepository.GetRegistrationCountAsync(request.ProgramId, cancellationToken);
        var hasCapacity = await _capacityManagementService.HasAvailableCapacityAsync(currentCount, null);

        var status = await _registrationEngine.DetermineInitialStatusAsync(
            request.ProgramType, request.RegistrationType, cancellationToken);

        if (status == PlatformRegistrationStatus.Waitlisted)
        {
            if (!hasCapacity)
            {
                var waitlistStatus = await _waitlistEngine.DetermineWaitlistStatusAsync(false, true);
                if (waitlistStatus == WaitlistStatus.Active)
                {
                    var waitlistCount = await _registrationRepository.GetRegistrationCountAsync(request.ProgramId, cancellationToken);
                    var position = await _capacityManagementService.CalculateNextWaitlistPositionAsync(waitlistCount);
                    _logger.LogInformation("Adding to waitlist at position {Position}", position);
                }
            }
        }

        var registrationNumber = await _registrationEngine.GenerateRegistrationNumberAsync(request.ProgramType, cancellationToken);

        var registration = new Domain.Entities.EventRegistration
        {
            Id = Guid.NewGuid(),
            EventId = request.ProgramId,
            AthleteId = request.AthleteId,
            UserId = request.UserId,
            RegistrationNumber = registrationNumber,
            Status = MapToEventStatus(status),
            AmountPaid = request.AmountPaid,
            PaymentReference = request.PaymentReference,
            Notes = request.Notes,
            RegistrationDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _registrationRepository.AddAsync(registration, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new PlatformRegistrationDto
        {
            Id = registration.Id,
            ProgramType = request.ProgramType,
            ProgramId = registration.EventId,
            AthleteId = registration.AthleteId,
            UserId = registration.UserId,
            RegistrationNumber = registration.RegistrationNumber,
            Status = status,
            AmountPaid = registration.AmountPaid,
            PaymentReference = registration.PaymentReference,
            Notes = registration.Notes,
            RegistrationDate = registration.RegistrationDate,
            CreatedAt = registration.CreatedAt
        };

        _logger.LogInformation("Registration completed: {RegistrationNumber} with status {Status}", registrationNumber, status);
        return Result<PlatformRegistrationDto>.Success(dto);
    }

    private static EventRegistrationStatus MapToEventStatus(PlatformRegistrationStatus status) => status switch
    {
        PlatformRegistrationStatus.Pending => EventRegistrationStatus.Pending,
        PlatformRegistrationStatus.Approved => EventRegistrationStatus.Approved,
        PlatformRegistrationStatus.Rejected => EventRegistrationStatus.Rejected,
        PlatformRegistrationStatus.Waitlisted => EventRegistrationStatus.Waitlisted,
        PlatformRegistrationStatus.Cancelled => EventRegistrationStatus.Cancelled,
        _ => EventRegistrationStatus.Pending
    };
}
