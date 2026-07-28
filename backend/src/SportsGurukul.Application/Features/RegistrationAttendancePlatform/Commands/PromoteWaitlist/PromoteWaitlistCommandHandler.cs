using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.PromoteWaitlist;

public class PromoteWaitlistCommandHandler : IRequestHandler<PromoteWaitlistCommand, Result<PlatformRegistrationDto>>
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IWaitlistEngine _waitlistEngine;
    private readonly ICapacityManagementService _capacityManagementService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PromoteWaitlistCommandHandler> _logger;

    public PromoteWaitlistCommandHandler(
        IEventRegistrationRepository registrationRepository,
        IWaitlistEngine waitlistEngine,
        ICapacityManagementService capacityManagementService,
        IUnitOfWork unitOfWork,
        ILogger<PromoteWaitlistCommandHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _waitlistEngine = waitlistEngine;
        _capacityManagementService = capacityManagementService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PlatformRegistrationDto>> Handle(PromoteWaitlistCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Promoting from waitlist for {ProgramType} {ProgramId}", request.ProgramType, request.ProgramId);

        var waitlisted = await _registrationRepository.GetByEventIdWithStatusAsync(
            request.ProgramId, EventRegistrationStatus.Waitlisted, cancellationToken);

        if (waitlisted.Count == 0)
            return Result<PlatformRegistrationDto>.Failure("No waitlisted registrations found.");

        var nextInLine = waitlisted.OrderBy(r => r.WaitlistPosition).First();

        var currentCount = await _registrationRepository.GetRegistrationCountAsync(request.ProgramId, cancellationToken);
        var hasCapacity = await _capacityManagementService.HasAvailableCapacityAsync(currentCount, null);

        var canPromote = await _waitlistEngine.CanPromoteAsync(WaitlistStatus.Active, hasCapacity);
        if (!canPromote)
            return Result<PlatformRegistrationDto>.Failure("Cannot promote from waitlist: no capacity available.");

        nextInLine.Status = EventRegistrationStatus.Approved;
        nextInLine.ApprovalDate = DateTime.UtcNow;
        nextInLine.WaitlistPosition = null;
        nextInLine.UpdatedAt = DateTime.UtcNow;

        _registrationRepository.Update(nextInLine);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new PlatformRegistrationDto
        {
            Id = nextInLine.Id,
            ProgramType = request.ProgramType,
            ProgramId = nextInLine.EventId,
            AthleteId = nextInLine.AthleteId,
            UserId = nextInLine.UserId,
            RegistrationNumber = nextInLine.RegistrationNumber,
            Status = PlatformRegistrationStatus.Approved,
            AmountPaid = nextInLine.AmountPaid,
            PaymentReference = nextInLine.PaymentReference,
            Notes = nextInLine.Notes,
            RegistrationDate = nextInLine.RegistrationDate,
            ApprovalDate = nextInLine.ApprovalDate,
            CreatedAt = nextInLine.CreatedAt
        };

        _logger.LogInformation("Promoted registration {RegistrationNumber} from waitlist", nextInLine.RegistrationNumber);
        return Result<PlatformRegistrationDto>.Success(dto);
    }
}
