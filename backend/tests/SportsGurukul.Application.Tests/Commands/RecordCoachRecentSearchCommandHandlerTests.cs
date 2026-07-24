using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.RecordCoachRecentSearch;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class RecordCoachRecentSearchCommandHandlerTests
{
    private readonly Mock<IRecentSearchRepository> _repositoryMock = TestMocks.CreateRecentSearchRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<RecordCoachRecentSearchCommandHandler>> _loggerMock = TestMocks.CreateLogger<RecordCoachRecentSearchCommandHandler>();
    private readonly RecordCoachRecentSearchCommandHandler _handler;

    public RecordCoachRecentSearchCommandHandlerTests()
    {
        _handler = new RecordCoachRecentSearchCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_RecordsSearchAndReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RecordCoachRecentSearchCommand
        {
            UserId = userId,
            QueryText = "cricket coaches",
            FiltersJson = "{\"city\":\"Mumbai\"}",
            ResultCount = 15
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<RecentSearch>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteOlderThanAsync(userId, 20, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsSearchedAtToUtcNow()
    {
        RecentSearch? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<RecentSearch>(), It.IsAny<CancellationToken>()))
            .Callback<RecentSearch, CancellationToken>((s, _) => captured = s)
            .Returns<RecentSearch, CancellationToken>((s, _) => Task.FromResult(s));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var before = DateTime.UtcNow;
        await _handler.Handle(new RecordCoachRecentSearchCommand
        {
            UserId = Guid.NewGuid(),
            QueryText = "tennis coaches",
            FiltersJson = "{}",
            ResultCount = 5
        }, CancellationToken.None);
        var after = DateTime.UtcNow;

        captured.Should().NotBeNull();
        captured!.SearchedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public async Task Handle_ValidCommand_CapsRecentSearchesAt20()
    {
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new RecordCoachRecentSearchCommand
        {
            UserId = userId,
            QueryText = "search",
            FiltersJson = "{}",
            ResultCount = 0
        }, CancellationToken.None);

        _repositoryMock.Verify(
            r => r.DeleteOlderThanAsync(userId, 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
