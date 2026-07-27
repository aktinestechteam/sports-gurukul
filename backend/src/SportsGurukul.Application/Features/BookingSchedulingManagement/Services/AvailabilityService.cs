using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<AvailabilityService> _logger;

    public AvailabilityService(
        IBookingRepository bookingRepository,
        IFacilityRepository facilityRepository,
        ICoachRepository coachRepository,
        IAthleteRepository athleteRepository,
        ILogger<AvailabilityService> logger)
    {
        _bookingRepository = bookingRepository;
        _facilityRepository = facilityRepository;
        _coachRepository = coachRepository;
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<bool> IsFacilityAvailableAsync(
        Guid facilityId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(facilityId, cancellationToken);
        if (facility is null || facility.IsDeleted)
        {
            _logger.LogWarning("Facility {FacilityId} not found or deleted", facilityId);
            return false;
        }

        if (facility.Status != FacilityStatus.Active)
        {
            _logger.LogInformation("Facility {FacilityId} is not active (Status: {Status})", facilityId, facility.Status);
            return false;
        }

        var bookings = await _bookingRepository
            .GetByFacilityIdAsync(facilityId, date, cancellationToken);

        var hasConflict = bookings.Any(b =>
            b.Id != excludeBookingId &&
            b.Status != BookingStatus.Cancelled &&
            b.Status != BookingStatus.Rejected &&
            b.StartTime < endTime &&
            b.EndTime > startTime);

        return !hasConflict;
    }

    public async Task<bool> IsCoachAvailableAsync(
        Guid coachId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default)
    {
        var coach = await _coachRepository.GetByIdAsync(coachId, cancellationToken);
        if (coach is null || coach.IsDeleted)
        {
            _logger.LogWarning("Coach {CoachId} not found or deleted", coachId);
            return false;
        }

        if (coach.Status != CoachStatus.Active)
        {
            _logger.LogInformation("Coach {CoachId} is not active (Status: {Status})", coachId, coach.Status);
            return false;
        }

        var bookings = await _bookingRepository
            .GetByCoachIdAsync(coachId, date, cancellationToken);

        var hasConflict = bookings.Any(b =>
            b.Id != excludeBookingId &&
            b.Status != BookingStatus.Cancelled &&
            b.Status != BookingStatus.Rejected &&
            b.StartTime < endTime &&
            b.EndTime > startTime);

        return !hasConflict;
    }

    public async Task<bool> IsAthleteAvailableAsync(
        Guid athleteId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default)
    {
        var athlete = await _athleteRepository.GetByIdAsync(athleteId, cancellationToken);
        if (athlete is null || athlete.IsDeleted)
        {
            _logger.LogWarning("Athlete {AthleteId} not found or deleted", athleteId);
            return false;
        }

        var bookings = await _bookingRepository
            .GetByAthleteIdAsync(athleteId, cancellationToken);

        var hasConflict = bookings.Any(b =>
            b.Id != excludeBookingId &&
            b.BookingDate.Date == date.Date &&
            b.Status != BookingStatus.Cancelled &&
            b.Status != BookingStatus.Rejected &&
            b.StartTime < endTime &&
            b.EndTime > startTime);

        return !hasConflict;
    }
}
