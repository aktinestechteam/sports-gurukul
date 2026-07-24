using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachAvailability;

public class GetCoachAvailabilityQueryHandler : IRequestHandler<GetCoachAvailabilityQuery, Result<AvailabilityDto>>
{
    private readonly ICoachAvailabilityRepository _coachAvailabilityRepository;
    private readonly ILogger<GetCoachAvailabilityQueryHandler> _logger;

    public GetCoachAvailabilityQueryHandler(
        ICoachAvailabilityRepository coachAvailabilityRepository,
        ILogger<GetCoachAvailabilityQueryHandler> logger)
    {
        _coachAvailabilityRepository = coachAvailabilityRepository;
        _logger = logger;
    }

    public async Task<Result<AvailabilityDto>> Handle(GetCoachAvailabilityQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting availability for coach Id: {CoachId}", request.CoachId);

        var availability = await _coachAvailabilityRepository.GetByCoachIdAsync(request.CoachId, cancellationToken);
        if (availability is null)
            return Result<AvailabilityDto>.Failure("Availability not found for the given coach.");

        var dto = new AvailabilityDto
        {
            Id = availability.Id,
            WeeklySchedule = availability.WeeklySchedule,
            TimeSlots = availability.TimeSlots,
            OnlineAvailable = availability.OnlineAvailable,
            OfflineAvailable = availability.OfflineAvailable,
            TravelDistance = availability.TravelDistance,
            CreatedAt = availability.CreatedAt,
            UpdatedAt = availability.UpdatedAt
        };

        return Result<AvailabilityDto>.Success(dto);
    }
}
