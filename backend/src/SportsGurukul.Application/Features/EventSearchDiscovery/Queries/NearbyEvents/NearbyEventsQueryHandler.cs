using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.NearbyEvents;

public class NearbyEventsQueryHandler : IRequestHandler<NearbyEventsQuery, Result<IReadOnlyList<NearbyEventDto>>>
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ILogger<NearbyEventsQueryHandler> _logger;

    public NearbyEventsQueryHandler(
        IEventSearchRepository searchRepository,
        ILogger<NearbyEventsQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<NearbyEventDto>>> Handle(NearbyEventsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting nearby events: Lat={Lat}, Lng={Lng}, Radius={Radius}km",
            request.Latitude, request.Longitude, request.RadiusKm);

        var events = await _searchRepository.GetNearbyEventsAsync(
            request.Latitude, request.Longitude, request.RadiusKm, request.Limit, cancellationToken);

        var items = events.Select(e =>
        {
            var primaryVenue = e.Venues?.FirstOrDefault(v => v.IsPrimary);
            var distance = CalculateDistance(
                request.Latitude, request.Longitude,
                primaryVenue?.Latitude ?? 0, primaryVenue?.Longitude ?? 0);

            return new NearbyEventDto
            {
                Id = e.Id,
                EventName = e.EventName,
                EventCode = e.EventCode,
                ShortDescription = e.ShortDescription,
                BannerUrl = e.BannerUrl,
                EventType = e.EventType?.Name,
                StartDate = e.StartDate,
                RegistrationFee = e.RegistrationFee,
                AcademyName = e.Academy?.Name ?? string.Empty,
                City = primaryVenue?.City,
                Latitude = primaryVenue?.Latitude,
                Longitude = primaryVenue?.Longitude,
                DistanceKm = Math.Round(distance, 2),
                IsRegistrationOpen = e.Status == Domain.Enums.EventStatus.RegistrationOpen
            };
        })
        .OrderBy(x => x.DistanceKm)
        .ToList();

        return Result<IReadOnlyList<NearbyEventDto>>.Success(items);
    }

    private static double CalculateDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2)
    {
        const double R = 6371;
        var dLat = ToRadians((double)(lat2 - lat1));
        var dLng = ToRadians((double)(lng2 - lng1));
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
