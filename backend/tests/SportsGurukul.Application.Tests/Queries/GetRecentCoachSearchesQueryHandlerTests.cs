using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetRecentCoachSearches;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetRecentCoachSearchesQueryHandlerTests
{
    private readonly Mock<IRecentSearchRepository> _repositoryMock = TestMocks.CreateRecentSearchRepository();
    private readonly Mock<ILogger<GetRecentCoachSearchesQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetRecentCoachSearchesQueryHandler>();
    private readonly GetRecentCoachSearchesQueryHandler _handler;

    public GetRecentCoachSearchesQueryHandlerTests()
    {
        _handler = new GetRecentCoachSearchesQueryHandler(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsRecentSearches()
    {
        var userId = Guid.NewGuid();
        var searches = new List<RecentSearch>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, QueryText = "cricket coaches", FiltersJson = "{}", ResultCount = 10, SearchedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = Guid.NewGuid(), UserId = userId, QueryText = "football coaches", FiltersJson = "{}", ResultCount = 5, SearchedAt = DateTime.UtcNow.AddHours(-2) }
        };
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searches);

        var result = await _handler.Handle(new GetRecentCoachSearchesQuery { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].QueryText.Should().Be("cricket coaches");
        result.Value[0].ResultCount.Should().Be(10);
    }

    [Fact]
    public async Task Handle_NoSearches_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RecentSearch>());

        var result = await _handler.Handle(new GetRecentCoachSearchesQuery { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CustomLimit_PassesLimitToRepository()
    {
        var userId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RecentSearch>());

        await _handler.Handle(new GetRecentCoachSearchesQuery { UserId = userId, Limit = 5 }, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByUserIdAsync(userId, 5, It.IsAny<CancellationToken>()), Times.Once);
    }
}
