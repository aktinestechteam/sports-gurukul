using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.RecordAcademySearch;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class RecordAcademySearchCommandHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<RecordAcademySearchCommandHandler>> _loggerMock;
    private readonly RecordAcademySearchCommandHandler _handler;

    public RecordAcademySearchCommandHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<RecordAcademySearchCommandHandler>>();
        _handler = new RecordAcademySearchCommandHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_RecordsSearch()
    {
        var userId = Guid.NewGuid();

        _academySearchRepositoryMock
            .Setup(r => r.RecordSearchAsync(It.IsAny<RecentAcademySearch>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(
            new RecordAcademySearchCommand
            {
                UserId = userId,
                SearchTerm = "cricket",
                City = "Mumbai",
                State = "Maharashtra",
                SportName = "Cricket",
                AcademyCount = 15
            },
            CancellationToken.None);

        _academySearchRepositoryMock.Verify(
            r => r.RecordSearchAsync(
                It.Is<RecentAcademySearch>(s =>
                    s.UserId == userId &&
                    s.SearchTerm == "cricket" &&
                    s.City == "Mumbai" &&
                    s.State == "Maharashtra" &&
                    s.SportName == "Cricket" &&
                    s.AcademyCount == 15),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
