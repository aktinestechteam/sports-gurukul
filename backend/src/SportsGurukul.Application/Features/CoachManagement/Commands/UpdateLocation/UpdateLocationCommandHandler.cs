using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateLocation;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Result<LocationDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IRepository<CoachLocation> _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateLocationCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public UpdateLocationCommandHandler(
        ICoachRepository coachRepository,
        IRepository<CoachLocation> locationRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateLocationCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachRepository = coachRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<LocationDto>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating location for coach: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<LocationDto>.Failure("Coach not found.");

        if (_currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<LocationDto>.Failure("You are not authorized to modify this coach's data.");

        if (request.Latitude.HasValue && (request.Latitude < -90 || request.Latitude > 90))
            return Result<LocationDto>.Failure("Latitude must be between -90 and 90.");

        if (request.Longitude.HasValue && (request.Longitude < -180 || request.Longitude > 180))
            return Result<LocationDto>.Failure("Longitude must be between -180 and 180.");

        var locations = await _locationRepository.FindAsync(
            l => l.CoachId == request.CoachId, cancellationToken);

        var location = locations.FirstOrDefault();

        if (location is null)
        {
            location = new CoachLocation
            {
                Id = Guid.NewGuid(),
                CoachId = request.CoachId,
                Country = request.Country,
                State = request.State,
                City = request.City,
                District = request.District,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            await _locationRepository.AddAsync(location, cancellationToken);
        }
        else
        {
            if (request.Country is not null)
                location.Country = request.Country;

            if (request.State is not null)
                location.State = request.State;

            if (request.City is not null)
                location.City = request.City;

            if (request.District is not null)
                location.District = request.District;

            if (request.Latitude.HasValue)
                location.Latitude = request.Latitude.Value;

            if (request.Longitude.HasValue)
                location.Longitude = request.Longitude.Value;

            location.UpdatedAt = DateTime.UtcNow;

            _locationRepository.Update(location);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Location updated for coach: {CoachId}", request.CoachId);

        var dto = new LocationDto
        {
            Id = location.Id,
            Country = location.Country,
            State = location.State,
            City = location.City,
            District = location.District,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };

        return Result<LocationDto>.Success(dto);
    }
}
