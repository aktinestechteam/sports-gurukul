using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class PersonalizationServiceTests
{
    private readonly Mock<ICacheService> _cacheServiceMock = new();
    private readonly Mock<ILogger<PersonalizationService>> _loggerMock = new();
    private readonly PersonalizationService _service;

    public PersonalizationServiceTests()
    {
        _service = new PersonalizationService(_cacheServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetUserPreferencesAsync_Cached_ReturnsCached()
    {
        var userId = Guid.NewGuid();
        var prefs = new UserPreferences { PreferredSports = ["Cricket"], PreferredCity = "Mumbai" };
        _cacheServiceMock.Setup(r => r.GetAsync<UserPreferences>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prefs);

        var result = await _service.GetUserPreferencesAsync(userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.PreferredSports.Should().Contain("Cricket");
        result.PreferredCity.Should().Be("Mumbai");
    }

    [Fact]
    public async Task GetUserPreferencesAsync_NotCached_ReturnsDefaults()
    {
        _cacheServiceMock.Setup(r => r.GetAsync<UserPreferences>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreferences?)null);

        var result = await _service.GetUserPreferencesAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().NotBeNull();
        result.PreferredSports.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateUserPreferencesAsync_SetsCache()
    {
        var userId = Guid.NewGuid();
        var prefs = new UserPreferences { PreferredCity = "Delhi" };

        await _service.UpdateUserPreferencesAsync(userId, prefs, CancellationToken.None);

        _cacheServiceMock.Verify(r => r.SetAsync(
            It.IsAny<string>(), prefs, TimeSpan.FromHours(24), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TrackInteractionAsync_AddsInteraction()
    {
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        await _service.TrackInteractionAsync(userId, eventId, "view", CancellationToken.None);

        _cacheServiceMock.Verify(r => r.SetAsync(
            It.IsAny<string>(), It.IsAny<List<UserInteraction>>(),
            It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
