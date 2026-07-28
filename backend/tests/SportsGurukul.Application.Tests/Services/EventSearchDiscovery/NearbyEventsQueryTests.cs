using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.NearbyEvents;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class NearbyEventsQueryTests
{
    private readonly Mock<IEventSearchRepository> _searchRepositoryMock = new();
    private readonly Mock<ILogger<NearbyEventsQueryHandler>> _loggerMock = new();
    private readonly NearbyEventsQueryHandler _handler;

    public NearbyEventsQueryTests()
    {
        _handler = new NearbyEventsQueryHandler(_searchRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoEvents_ReturnsEmpty()
    {
        _searchRepositoryMock.Setup(r => r.GetNearbyEventsAsync(
            It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>(),
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Event>());

        var result = await _handler.Handle(new NearbyEventsQuery
        {
            Latitude = 19.0760m,
            Longitude = 72.8777m,
            RadiusKm = 10,
            Limit = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithEvents_ReturnsSortedByDistance()
    {
        var events = new List<Domain.Entities.Event>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EventName = "Far Event",
                EventCode = "EVT-FAR",
                Status = Domain.Enums.EventStatus.RegistrationOpen,
                Venues = new List<Domain.Entities.EventVenue>
                {
                    new() { City = "Pune", Latitude = 18.5204m, Longitude = 73.8567m, IsPrimary = true }
                },
                Registrations = new List<Domain.Entities.EventRegistration>(),
                Feedbacks = new List<Domain.Entities.EventFeedback>()
            },
            new()
            {
                Id = Guid.NewGuid(),
                EventName = "Near Event",
                EventCode = "EVT-NEAR",
                Status = Domain.Enums.EventStatus.RegistrationOpen,
                Venues = new List<Domain.Entities.EventVenue>
                {
                    new() { City = "Mumbai", Latitude = 19.0760m, Longitude = 72.8777m, IsPrimary = true }
                },
                Registrations = new List<Domain.Entities.EventRegistration>(),
                Feedbacks = new List<Domain.Entities.EventFeedback>()
            }
        };

        _searchRepositoryMock.Setup(r => r.GetNearbyEventsAsync(
            It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>(),
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        var result = await _handler.Handle(new NearbyEventsQuery
        {
            Latitude = 19.0760m,
            Longitude = 72.8777m,
            RadiusKm = 500,
            Limit = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].DistanceKm.Should().BeLessThan(result.Value[1].DistanceKm);
    }
}
