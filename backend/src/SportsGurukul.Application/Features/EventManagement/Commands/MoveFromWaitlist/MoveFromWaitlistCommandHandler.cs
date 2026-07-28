using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.MoveFromWaitlist;

public class MoveFromWaitlistCommandHandler : IRequestHandler<MoveFromWaitlistCommand, Result<RegistrationDto>>
{
    private readonly IEventRegistrationRepository _eventRegistrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MoveFromWaitlistCommandHandler> _logger;

    public MoveFromWaitlistCommandHandler(
        IEventRegistrationRepository eventRegistrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<MoveFromWaitlistCommandHandler> logger)
    {
        _eventRegistrationRepository = eventRegistrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegistrationDto>> Handle(MoveFromWaitlistCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Moving registration {RegistrationId} from waitlist", request.RegistrationId);

        var registration = await _eventRegistrationRepository.GetWithDetailsAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
        {
            _logger.LogWarning("Registration {RegistrationId} not found", request.RegistrationId);
            return Result<RegistrationDto>.Failure("Registration not found");
        }

        if (registration.Status != EventRegistrationStatus.Waitlisted)
        {
            _logger.LogWarning("Registration {RegistrationId} is not waitlisted. Current status: {Status}", request.RegistrationId, registration.Status);
            return Result<RegistrationDto>.Failure("Only waitlisted registrations can be moved from waitlist");
        }

        registration.Status = EventRegistrationStatus.Approved;
        registration.ApprovalDate = DateTime.UtcNow;
        registration.UpdatedAt = DateTime.UtcNow;
        _eventRegistrationRepository.Update(registration);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = RegistrationDtoExtensions.MapToDto(registration, registration.Event?.EventName ?? string.Empty);

        _logger.LogInformation("Registration {RegistrationId} moved from waitlist and approved successfully", request.RegistrationId);
        return Result<RegistrationDto>.Success(dto);
    }
}
