using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetRegistrationStatus;

public class GetRegistrationStatusQueryHandler : IRequestHandler<GetRegistrationStatusQuery, Result<PlatformRegistrationDto>>
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly ILogger<GetRegistrationStatusQueryHandler> _logger;

    public GetRegistrationStatusQueryHandler(
        IEventRegistrationRepository registrationRepository,
        ILogger<GetRegistrationStatusQueryHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<Result<PlatformRegistrationDto>> Handle(GetRegistrationStatusQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching registration status for {RegistrationId}", request.RegistrationId);

        var registration = await _registrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
            return Result<PlatformRegistrationDto>.Failure("Registration not found.");

        var status = registration.Status switch
        {
            EventRegistrationStatus.Pending => PlatformRegistrationStatus.Pending,
            EventRegistrationStatus.Approved => PlatformRegistrationStatus.Approved,
            EventRegistrationStatus.Rejected => PlatformRegistrationStatus.Rejected,
            EventRegistrationStatus.Waitlisted => PlatformRegistrationStatus.Waitlisted,
            EventRegistrationStatus.Cancelled => PlatformRegistrationStatus.Cancelled,
            _ => PlatformRegistrationStatus.Pending
        };

        var dto = new PlatformRegistrationDto
        {
            Id = registration.Id,
            ProgramId = registration.EventId,
            AthleteId = registration.AthleteId,
            UserId = registration.UserId,
            RegistrationNumber = registration.RegistrationNumber,
            Status = status,
            AmountPaid = registration.AmountPaid,
            PaymentReference = registration.PaymentReference,
            Notes = registration.Notes,
            RegistrationDate = registration.RegistrationDate,
            ApprovalDate = registration.ApprovalDate,
            RejectionReason = registration.RejectionReason,
            WaitlistPosition = registration.WaitlistPosition,
            CreatedAt = registration.CreatedAt
        };

        _logger.LogInformation("Registration {RegistrationId} status: {Status}", request.RegistrationId, status);
        return Result<PlatformRegistrationDto>.Success(dto);
    }
}
