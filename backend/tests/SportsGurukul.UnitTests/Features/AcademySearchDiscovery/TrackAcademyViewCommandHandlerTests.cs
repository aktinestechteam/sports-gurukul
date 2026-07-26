using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.TrackAcademyView;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class TrackAcademyViewCommandHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<TrackAcademyViewCommandHandler>> _loggerMock;
    private readonly TrackAcademyViewCommandHandler _handler;

    public TrackAcademyViewCommandHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<TrackAcademyViewCommandHandler>>();
        _handler = new TrackAcademyViewCommandHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_TracksView()
    {
        var academyId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _academySearchRepositoryMock
            .Setup(r => r.TrackViewAsync(It.IsAny<AcademyView>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(
            new TrackAcademyViewCommand
            {
                AcademyId = academyId,
                UserId = userId,
                Source = "search_results"
            },
            CancellationToken.None);

        _academySearchRepositoryMock.Verify(
            r => r.TrackViewAsync(
                It.Is<AcademyView>(v =>
                    v.AcademyId == academyId &&
                    v.ViewedByUserId == userId &&
                    v.Source == "search_results"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
