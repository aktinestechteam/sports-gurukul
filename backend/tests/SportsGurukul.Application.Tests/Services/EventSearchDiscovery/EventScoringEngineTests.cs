using FluentAssertions;
using Moq;
using SportsGurukul.Application.Features.EventSearchDiscovery.Engines;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class EventScoringEngineTests
{
    private readonly EventScoringEngine _engine;
    private readonly Mock<ILogger<EventScoringEngine>> _loggerMock = new();

    public EventScoringEngineTests()
    {
        _engine = new EventScoringEngine(_loggerMock.Object);
    }

    [Fact]
    public async Task ScoreEventsAsync_FeaturedEvent_GetsHigherScore()
    {
        var events = new List<Event>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EventName = "Featured Event",
                Status = EventStatus.RegistrationOpen,
                StartDate = DateTime.UtcNow.AddDays(10),
                IsFeatured = true,
                IsPublic = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                EventName = "Regular Event",
                Status = EventStatus.RegistrationOpen,
                StartDate = DateTime.UtcNow.AddDays(10),
                IsFeatured = false,
                IsPublic = true
            }
        };

        var scores = await _engine.ScoreEventsAsync(events, null, [], [], CancellationToken.None);

        scores.Should().HaveCount(2);
        var featured = scores.First(s => s.EventId == events[0].Id);
        var regular = scores.First(s => s.EventId == events[1].Id);
        featured.Score.Should().BeGreaterThan(regular.Score);
    }

    [Fact]
    public async Task ScoreEventsAsync_FreeEvent_GetsBonusScore()
    {
        var events = new List<Event>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EventName = "Free Event",
                Status = EventStatus.RegistrationOpen,
                StartDate = DateTime.UtcNow.AddDays(10),
                RegistrationFee = null,
                IsFeatured = false,
                IsPublic = true
            }
        };

        var scores = await _engine.ScoreEventsAsync(events, null, [], [], CancellationToken.None);

        scores.Should().HaveCount(1);
        scores[0].Score.Should().BeGreaterThan(0);
        scores[0].Reason.Should().Contain("Free event");
    }

    [Fact]
    public async Task ScoreEventsAsync_StartingSoon_GetsTimelyBonus()
    {
        var events = new List<Event>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EventName = "Starting Soon Event",
                Status = EventStatus.RegistrationOpen,
                StartDate = DateTime.UtcNow.AddDays(5),
                IsFeatured = false,
                IsPublic = true
            }
        };

        var scores = await _engine.ScoreEventsAsync(events, null, [], [], CancellationToken.None);

        scores.Should().HaveCount(1);
        scores[0].Reason.Should().Contain("Starting soon");
    }

    [Fact]
    public async Task StrategyName_ReturnsCorrectName()
    {
        _engine.StrategyName.Should().Be("EventAttributeScoring");
    }

    [Fact]
    public async Task Priority_Returns1()
    {
        _engine.Priority.Should().Be(1);
    }
}
