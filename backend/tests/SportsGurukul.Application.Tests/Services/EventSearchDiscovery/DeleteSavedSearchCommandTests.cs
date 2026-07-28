using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.DeleteSavedSearch;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class DeleteSavedSearchCommandTests
{
    private readonly Mock<IEventSearchRepository> _searchRepositoryMock = new();
    private readonly Mock<ILogger<DeleteSavedSearchCommandHandler>> _loggerMock = new();
    private readonly DeleteSavedSearchCommandHandler _handler;

    public DeleteSavedSearchCommandTests()
    {
        _handler = new DeleteSavedSearchCommandHandler(_searchRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidIds_DeletesSuccessfully()
    {
        var command = new DeleteSavedSearchCommand
        {
            SavedSearchId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _searchRepositoryMock.Setup(r => r.DeleteSavedSearchAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        var command = new DeleteSavedSearchCommand
        {
            SavedSearchId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _searchRepositoryMock.Setup(r => r.DeleteSavedSearchAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Saved search not found or access denied."));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
