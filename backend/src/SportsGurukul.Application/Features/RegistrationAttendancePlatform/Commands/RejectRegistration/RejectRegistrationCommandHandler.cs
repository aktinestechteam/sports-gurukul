using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RejectRegistration;

public class RejectRegistrationCommandHandler : IRequestHandler<RejectRegistrationCommand, Result<PlatformRegistrationDto>>
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectRegistrationCommandHandler> _logger;

    public RejectRegistrationCommandHandler(
        IEventRegistrationRepository registrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<RejectRegistrationCommandHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PlatformRegistrationDto>> Handle(RejectRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting registration {RegistrationId}", request.RegistrationId);

        var registration = await _registrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
            return Result<PlatformRegistrationDto>.Failure("Registration not found.");

        if (registration.Status != EventRegistrationStatus.Pending)
            return Result<PlatformRegistrationDto>.Failure("Only pending registrations can be rejected.");

        registration.Status = EventRegistrationStatus.Rejected;
        registration.RejectionReason = request.Reason;
        registration.UpdatedAt = DateTime.UtcNow;

        _registrationRepository.Update(registration);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new PlatformRegistrationDto
        {
            Id = registration.Id,
            ProgramId = registration.EventId,
            AthleteId = registration.AthleteId,
            UserId = registration.UserId,
            RegistrationNumber = registration.RegistrationNumber,
            Status = PlatformRegistrationStatus.Rejected,
            RejectionReason = registration.RejectionReason,
            RegistrationDate = registration.RegistrationDate,
            CreatedAt = registration.CreatedAt
        };

        _logger.LogInformation("Registration {RegistrationId} rejected by {RejectedBy}. Reason: {Reason}", request.RegistrationId, request.RejectedBy, request.Reason);
        return Result<PlatformRegistrationDto>.Success(dto);
    }
}
