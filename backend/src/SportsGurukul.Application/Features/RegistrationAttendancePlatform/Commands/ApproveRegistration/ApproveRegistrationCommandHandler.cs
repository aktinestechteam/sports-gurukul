using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.ApproveRegistration;

public class ApproveRegistrationCommandHandler : IRequestHandler<ApproveRegistrationCommand, Result<PlatformRegistrationDto>>
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveRegistrationCommandHandler> _logger;

    public ApproveRegistrationCommandHandler(
        IEventRegistrationRepository registrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<ApproveRegistrationCommandHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PlatformRegistrationDto>> Handle(ApproveRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving registration {RegistrationId}", request.RegistrationId);

        var registration = await _registrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
            return Result<PlatformRegistrationDto>.Failure("Registration not found.");

        if (registration.Status != EventRegistrationStatus.Pending)
            return Result<PlatformRegistrationDto>.Failure("Only pending registrations can be approved.");

        registration.Status = EventRegistrationStatus.Approved;
        registration.ApprovalDate = DateTime.UtcNow;
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
            Status = PlatformRegistrationStatus.Approved,
            AmountPaid = registration.AmountPaid,
            PaymentReference = registration.PaymentReference,
            Notes = registration.Notes,
            RegistrationDate = registration.RegistrationDate,
            ApprovalDate = registration.ApprovalDate,
            CreatedAt = registration.CreatedAt
        };

        _logger.LogInformation("Registration {RegistrationId} approved by {ApprovedBy}", request.RegistrationId, request.ApprovedBy);
        return Result<PlatformRegistrationDto>.Success(dto);
    }
}
