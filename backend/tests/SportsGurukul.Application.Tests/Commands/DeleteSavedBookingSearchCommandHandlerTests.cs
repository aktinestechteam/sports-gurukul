using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.DeleteSavedBookingSearch;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class DeleteSavedBookingSearchCommandHandlerTests
{
    private readonly Mock<ISavedSearchRepository> _savedSearchRepositoryMock = TestMocks.CreateSavedSearchRepository();
    private readonly Mock<ILogger<DeleteSavedBookingSearchCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeleteSavedBookingSearchCommandHandler>();
    private readonly DeleteSavedBookingSearchCommandHandler _handler;

    public DeleteSavedBookingSearchCommandHandlerTests()
    {
        _handler = new DeleteSavedBookingSearchCommandHandler(
            _savedSearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SearchExists_DeletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var searchId = Guid.NewGuid();
        var search = new SavedSearch { Id = searchId, UserId = userId, Name = "Test" };

        _savedSearchRepositoryMock.Setup(r => r.GetByIdAndUserAsync(searchId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(search);

        var result = await _handler.Handle(new DeleteSavedBookingSearchCommand
        {
            UserId = userId,
            SavedSearchId = searchId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _savedSearchRepositoryMock.Verify(r => r.Remove(search), Times.Once);
    }

    [Fact]
    public async Task Handle_SearchNotFound_ReturnsFailure()
    {
        _savedSearchRepositoryMock.Setup(r => r.GetByIdAndUserAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SavedSearch?)null);

        var result = await _handler.Handle(new DeleteSavedBookingSearchCommand
        {
            UserId = Guid.NewGuid(),
            SavedSearchId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_DifferentUser_ReturnsFailure()
    {
        var search = new SavedSearch { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Other User's" };

        _savedSearchRepositoryMock.Setup(r => r.GetByIdAndUserAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SavedSearch?)null);

        var result = await _handler.Handle(new DeleteSavedBookingSearchCommand
        {
            UserId = Guid.NewGuid(),
            SavedSearchId = search.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}
