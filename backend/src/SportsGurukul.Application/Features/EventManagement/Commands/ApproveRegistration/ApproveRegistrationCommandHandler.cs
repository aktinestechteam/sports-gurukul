using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ApproveRegistration;

public class ApproveRegistrationCommandHandler : IRequestHandler<ApproveRegistrationCommand, Result<RegistrationDto>>
{
    private readonly IEventRegistrationRepository _eventRegistrationRepository;
    private readonly IEventRegistrationService _eventRegistrationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveRegistrationCommandHandler> _logger;

    public ApproveRegistrationCommandHandler(
        IEventRegistrationRepository eventRegistrationRepository,
        IEventRegistrationService eventRegistrationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveRegistrationCommandHandler> logger)
    {
        _eventRegistrationRepository = eventRegistrationRepository;
        _eventRegistrationService = eventRegistrationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegistrationDto>> Handle(ApproveRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving registration {RegistrationId}", request.RegistrationId);

        var registration = await _eventRegistrationRepository.GetWithDetailsAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
        {
            _logger.LogWarning("Registration {RegistrationId} not found", request.RegistrationId);
            return Result<RegistrationDto>.Failure("Registration not found");
        }

        if (registration.Status == EventRegistrationStatus.Waitlisted)
        {
            var hasCapacity = await _eventRegistrationService.IsCapacityAvailableAsync(registration.Event, cancellationToken);
            if (!hasCapacity)
            {
                _logger.LogWarning("No capacity available to approve waitlisted registration {RegistrationId}", request.RegistrationId);
                return Result<RegistrationDto>.Failure("Event has reached maximum capacity");
            }
        }
        else if (registration.Status != EventRegistrationStatus.Pending)
        {
            _logger.LogWarning("Registration {RegistrationId} cannot be approved. Current status: {Status}", request.RegistrationId, registration.Status);
            return Result<RegistrationDto>.Failure("Only pending or waitlisted registrations can be approved");
        }

        registration.Status = EventRegistrationStatus.Approved;
        registration.ApprovalDate = DateTime.UtcNow;
        registration.UpdatedAt = DateTime.UtcNow;
        _eventRegistrationRepository.Update(registration);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = RegistrationDtoExtensions.MapToDto(registration, registration.Event?.EventName ?? string.Empty);

        _logger.LogInformation("Registration {RegistrationId} approved successfully", request.RegistrationId);
        return Result<RegistrationDto>.Success(dto);
    }
}
