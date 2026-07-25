using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateAvailability;

public class UpdateAvailabilityCommandHandler : IRequestHandler<UpdateAvailabilityCommand, Result<AvailabilityDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ICoachAvailabilityRepository _availabilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAvailabilityCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public UpdateAvailabilityCommandHandler(
        ICoachRepository coachRepository,
        ICoachAvailabilityRepository availabilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAvailabilityCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachRepository = coachRepository;
        _availabilityRepository = availabilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<AvailabilityDto>> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating availability for coach: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<AvailabilityDto>.Failure("Coach not found.");

        if (_currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<AvailabilityDto>.Failure("You are not authorized to modify this coach's data.");

        var availability = await _availabilityRepository.GetByCoachIdAsync(request.CoachId, cancellationToken);

        if (availability is null)
        {
            availability = new CoachAvailability
            {
                Id = Guid.NewGuid(),
                CoachId = request.CoachId,
                WeeklySchedule = request.WeeklySchedule ?? string.Empty,
                TimeSlots = request.TimeSlots ?? string.Empty,
                OnlineAvailable = request.OnlineAvailable ?? true,
                OfflineAvailable = request.OfflineAvailable ?? true,
                TravelDistance = request.TravelDistance
            };

            await _availabilityRepository.AddAsync(availability, cancellationToken);
        }
        else
        {
            if (request.WeeklySchedule is not null)
                availability.WeeklySchedule = request.WeeklySchedule;

            if (request.TimeSlots is not null)
                availability.TimeSlots = request.TimeSlots;

            if (request.OnlineAvailable.HasValue)
                availability.OnlineAvailable = request.OnlineAvailable.Value;

            if (request.OfflineAvailable.HasValue)
                availability.OfflineAvailable = request.OfflineAvailable.Value;

            if (request.TravelDistance.HasValue)
                availability.TravelDistance = request.TravelDistance.Value;

            availability.UpdatedAt = DateTime.UtcNow;

            _availabilityRepository.Update(availability);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Availability updated for coach: {CoachId}", request.CoachId);

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
