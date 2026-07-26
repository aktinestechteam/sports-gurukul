using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.DeleteSavedAcademySearch;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class DeleteSavedAcademySearchCommandHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<DeleteSavedAcademySearchCommandHandler>> _loggerMock;
    private readonly DeleteSavedAcademySearchCommandHandler _handler;

    public DeleteSavedAcademySearchCommandHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<DeleteSavedAcademySearchCommandHandler>>();
        _handler = new DeleteSavedAcademySearchCommandHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidIds_DeletesSearch()
    {
        var searchId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _academySearchRepositoryMock
            .Setup(r => r.DeleteSavedSearchAsync(searchId, userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new DeleteSavedAcademySearchCommand
            {
                SearchId = searchId,
                UserId = userId
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _academySearchRepositoryMock.Verify(
            r => r.DeleteSavedSearchAsync(searchId, userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
