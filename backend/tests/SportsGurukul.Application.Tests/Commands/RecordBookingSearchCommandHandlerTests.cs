using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.RecordBookingSearch;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class RecordBookingSearchCommandHandlerTests
{
    private readonly Mock<IRecentSearchRepository> _recentSearchRepositoryMock = TestMocks.CreateRecentSearchRepository();
    private readonly Mock<ILogger<RecordBookingSearchCommandHandler>> _loggerMock = TestMocks.CreateLogger<RecordBookingSearchCommandHandler>();
    private readonly RecordBookingSearchCommandHandler _handler;

    public RecordBookingSearchCommandHandlerTests()
    {
        _handler = new RecordBookingSearchCommandHandler(
            _recentSearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_RecordsSearchAndReturnsSuccess()
    {
        RecentSearch? captured = null;
        _recentSearchRepositoryMock.Setup(r => r.AddAsync(
            It.IsAny<RecentSearch>(), It.IsAny<CancellationToken>()))
            .Callback<RecentSearch, CancellationToken>((s, _) => captured = s)
            .ReturnsAsync((RecentSearch s, CancellationToken _) =>
            {
                s.Id = Guid.NewGuid();
                return s;
            });
        _recentSearchRepositoryMock.Setup(r => r.DeleteOlderThanAsync(
            It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new RecordBookingSearchCommand
        {
            UserId = Guid.NewGuid(),
            SearchTerm = "morning training",
            ResultCount = 5
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.QueryText.Should().Be("morning training");
        captured.ResultCount.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsSearchedAtToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        _recentSearchRepositoryMock.Setup(r => r.AddAsync(
            It.IsAny<RecentSearch>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RecentSearch s, CancellationToken _) =>
            {
                s.Id = Guid.NewGuid();
                return s;
            });
        _recentSearchRepositoryMock.Setup(r => r.DeleteOlderThanAsync(
            It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(new RecordBookingSearchCommand
        {
            UserId = Guid.NewGuid(),
            SearchTerm = "test",
            ResultCount = 0
        }, CancellationToken.None);

        _recentSearchRepositoryMock.Verify(r => r.AddAsync(
            It.Is<RecentSearch>(s => s.SearchedAt >= before),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_CapsRecentSearchesAt20()
    {
        _recentSearchRepositoryMock.Setup(r => r.AddAsync(
            It.IsAny<RecentSearch>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RecentSearch s, CancellationToken _) =>
            {
                s.Id = Guid.NewGuid();
                return s;
            });
        _recentSearchRepositoryMock.Setup(r => r.DeleteOlderThanAsync(
            It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(new RecordBookingSearchCommand
        {
            UserId = Guid.NewGuid(),
            SearchTerm = "test",
            ResultCount = 0
        }, CancellationToken.None);

        _recentSearchRepositoryMock.Verify(r => r.DeleteOlderThanAsync(
            It.IsAny<Guid>(), 20, It.IsAny<CancellationToken>()), Times.Once);
    }
}
