using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.TrackRecentlyViewed;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class TrackRecentlyViewedCommandTests
{
    private readonly Mock<IEventSearchRepository> _searchRepositoryMock = new();
    private readonly Mock<ILogger<TrackRecentlyViewedCommandHandler>> _loggerMock = new();
    private readonly TrackRecentlyViewedCommandHandler _handler;

    public TrackRecentlyViewedCommandTests()
    {
        _handler = new TrackRecentlyViewedCommandHandler(_searchRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_TracksView()
    {
        var command = new TrackRecentlyViewedCommand
        {
            EventId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Source = "search",
            DeviceType = "mobile"
        };

        _searchRepositoryMock.Setup(r => r.TrackViewAsync(
            It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _searchRepositoryMock.Verify(r => r.TrackViewAsync(
            command.EventId, command.UserId, "search", "mobile", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AnonymousUser_TracksViewWithNullUserId()
    {
        var command = new TrackRecentlyViewedCommand
        {
            EventId = Guid.NewGuid(),
            UserId = null,
            Source = "api"
        };

        _searchRepositoryMock.Setup(r => r.TrackViewAsync(
            It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
