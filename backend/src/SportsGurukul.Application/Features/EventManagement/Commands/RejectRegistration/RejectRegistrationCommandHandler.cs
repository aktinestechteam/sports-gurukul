using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RejectRegistration;

public class RejectRegistrationCommandHandler : IRequestHandler<RejectRegistrationCommand, Result<RegistrationDto>>
{
    private readonly IEventRegistrationRepository _eventRegistrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectRegistrationCommandHandler> _logger;

    public RejectRegistrationCommandHandler(
        IEventRegistrationRepository eventRegistrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<RejectRegistrationCommandHandler> logger)
    {
        _eventRegistrationRepository = eventRegistrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegistrationDto>> Handle(RejectRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting registration {RegistrationId}", request.RegistrationId);

        var registration = await _eventRegistrationRepository.GetWithDetailsAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
        {
            _logger.LogWarning("Registration {RegistrationId} not found", request.RegistrationId);
            return Result<RegistrationDto>.Failure("Registration not found");
        }

        if (registration.Status != EventRegistrationStatus.Pending && registration.Status != EventRegistrationStatus.Waitlisted)
        {
            _logger.LogWarning("Registration {RegistrationId} cannot be rejected. Current status: {Status}", request.RegistrationId, registration.Status);
            return Result<RegistrationDto>.Failure("Only pending or waitlisted registrations can be rejected");
        }

        registration.Status = EventRegistrationStatus.Rejected;
        registration.RejectionReason = request.Reason;
        registration.UpdatedAt = DateTime.UtcNow;
        _eventRegistrationRepository.Update(registration);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = RegistrationDtoExtensions.MapToDto(registration, registration.Event?.EventName ?? string.Empty);

        _logger.LogInformation("Registration {RegistrationId} rejected successfully", request.RegistrationId);
        return Result<RegistrationDto>.Success(dto);
    }
}
