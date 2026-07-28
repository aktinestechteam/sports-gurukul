using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CancelRegistration;

public class CancelRegistrationCommandHandler : IRequestHandler<CancelRegistrationCommand, Result<RegistrationDto>>
{
    private readonly IEventRegistrationRepository _eventRegistrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelRegistrationCommandHandler> _logger;

    public CancelRegistrationCommandHandler(
        IEventRegistrationRepository eventRegistrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelRegistrationCommandHandler> logger)
    {
        _eventRegistrationRepository = eventRegistrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegistrationDto>> Handle(CancelRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling registration {RegistrationId}", request.RegistrationId);

        var registration = await _eventRegistrationRepository.GetWithDetailsAsync(request.RegistrationId, cancellationToken);
        if (registration is null)
        {
            _logger.LogWarning("Registration {RegistrationId} not found", request.RegistrationId);
            return Result<RegistrationDto>.Failure("Registration not found");
        }

        if (registration.Status == EventRegistrationStatus.Cancelled)
        {
            _logger.LogWarning("Registration {RegistrationId} is already cancelled", request.RegistrationId);
            return Result<RegistrationDto>.Failure("Registration is already cancelled");
        }

        registration.Status = EventRegistrationStatus.Cancelled;
        registration.UpdatedAt = DateTime.UtcNow;
        _eventRegistrationRepository.Update(registration);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = RegistrationDtoExtensions.MapToDto(registration, registration.Event?.EventName ?? string.Empty);

        _logger.LogInformation("Registration {RegistrationId} cancelled successfully", request.RegistrationId);
        return Result<RegistrationDto>.Success(dto);
    }
}
