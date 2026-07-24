using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteSavedCoachSearch;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class DeleteSavedCoachSearchCommandHandlerTests
{
    private readonly Mock<ISavedSearchRepository> _repositoryMock = TestMocks.CreateSavedSearchRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<DeleteSavedCoachSearchCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeleteSavedCoachSearchCommandHandler>();
    private readonly DeleteSavedCoachSearchCommandHandler _handler;

    public DeleteSavedCoachSearchCommandHandlerTests()
    {
        _handler = new DeleteSavedCoachSearchCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SearchNotFound_ReturnsFailure()
    {
        _repositoryMock.Setup(r => r.GetByIdAndUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SavedSearch?)null);

        var result = await _handler.Handle(new DeleteSavedCoachSearchCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Saved search not found.");
    }

    [Fact]
    public async Task Handle_SearchExists_DeletesAndReturnsSuccess()
    {
        var searchId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var savedSearch = new SavedSearch
        {
            Id = searchId,
            UserId = userId,
            Name = "My Coach Search",
            FiltersJson = "{}"
        };

        _repositoryMock.Setup(r => r.GetByIdAndUserAsync(searchId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedSearch);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteSavedCoachSearchCommand
        {
            Id = searchId,
            UserId = userId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.Remove(savedSearch), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SearchBelongsToDifferentUser_ReturnsFailure()
    {
        var searchId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAndUserAsync(searchId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SavedSearch?)null);

        var result = await _handler.Handle(new DeleteSavedCoachSearchCommand
        {
            Id = searchId,
            UserId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Saved search not found.");
    }
}
