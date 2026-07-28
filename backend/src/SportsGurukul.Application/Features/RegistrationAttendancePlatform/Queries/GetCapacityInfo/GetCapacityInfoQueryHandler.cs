using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetCapacityInfo;

public class GetCapacityInfoQueryHandler : IRequestHandler<GetCapacityInfoQuery, Result<PlatformCapacityDto>>
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly ICapacityManagementService _capacityManagementService;
    private readonly ILogger<GetCapacityInfoQueryHandler> _logger;

    public GetCapacityInfoQueryHandler(
        IEventRegistrationRepository registrationRepository,
        ICapacityManagementService capacityManagementService,
        ILogger<GetCapacityInfoQueryHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _capacityManagementService = capacityManagementService;
        _logger = logger;
    }

    public async Task<Result<PlatformCapacityDto>> Handle(GetCapacityInfoQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching capacity info for {ProgramType} {ProgramId}", request.ProgramType, request.ProgramId);

        var currentCount = await _registrationRepository.GetRegistrationCountAsync(request.ProgramId, cancellationToken);
        var availableSlots = await _capacityManagementService.GetAvailableSlotsAsync(currentCount, request.MaxCapacity);
        var isFull = await _capacityManagementService.IsAtCapacityAsync(currentCount, request.MaxCapacity);

        var dto = new PlatformCapacityDto
        {
            ProgramType = request.ProgramType,
            ProgramId = request.ProgramId,
            MaxCapacity = request.MaxCapacity,
            CurrentCount = currentCount,
            AvailableSlots = availableSlots == int.MaxValue ? 0 : availableSlots,
            IsFull = isFull,
            WaitlistCount = 0,
            WaitlistEnabled = request.WaitlistEnabled
        };

        _logger.LogInformation("Capacity info: {CurrentCount}/{MaxCapacity}, available: {Available}, full: {IsFull}",
            currentCount, request.MaxCapacity, availableSlots, isFull);
        return Result<PlatformCapacityDto>.Success(dto);
    }
}
