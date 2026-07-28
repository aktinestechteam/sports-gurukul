using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.Engines;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class RecommendationEngineTests
{
    private readonly Mock<IEventSearchRepository> _searchRepositoryMock = new();
    private readonly Mock<ILogger<RecommendationEngine>> _loggerMock = new();
    private readonly RecommendationEngine _engine;

    public RecommendationEngineTests()
    {
        var strategies = new List<IRecommendationStrategy>
        {
            new EventScoringEngine(new Mock<ILogger<EventScoringEngine>>().Object)
        };
        _engine = new RecommendationEngine(_searchRepositoryMock.Object, strategies, _loggerMock.Object);
    }

    [Fact]
    public async Task GetTrendingEventsAsync_ReturnsTrendingEvents()
    {
        var events = new List<Event>
        {
            new() { Id = Guid.NewGuid(), EventName = "Cricket Championship", EventCode = "EVT-001", Status = EventStatus.RegistrationOpen, StartDate = DateTime.UtcNow.AddDays(10), IsFeatured = true, RegistrationType = EventRegistrationType.Paid, IsPublic = true },
            new() { Id = Guid.NewGuid(), EventName = "Football Tournament", EventCode = "EVT-002", Status = EventStatus.Scheduled, StartDate = DateTime.UtcNow.AddDays(20), IsFeatured = false, RegistrationType = EventRegistrationType.Free, IsPublic = true }
        };
        _searchRepositoryMock.Setup(r => r.GetTrendingEventsAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        var result = await _engine.GetTrendingEventsAsync(null, 10, CancellationToken.None);

        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetFeaturedEventsAsync_ReturnsFeaturedEvents()
    {
        var events = new List<Event>
        {
            new() { Id = Guid.NewGuid(), EventName = "Featured Event", EventCode = "EVT-001", IsFeatured = true, Status = EventStatus.RegistrationOpen, StartDate = DateTime.UtcNow.AddDays(5), RegistrationFee = 500, IsPublic = true }
        };
        _searchRepositoryMock.Setup(r => r.GetFeaturedEventsAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        var result = await _engine.GetFeaturedEventsAsync(null, null, 10, CancellationToken.None);

        result.Should().NotBeNull();
        result.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetRecommendationsAsync_NoEvents_ReturnsEmpty()
    {
        _searchRepositoryMock.Setup(r => r.GetUpcomingEventsAsync(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Event>());

        var result = await _engine.GetRecommendationsAsync(null, null, [], [], null, null, 10, CancellationToken.None);

        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }
}
