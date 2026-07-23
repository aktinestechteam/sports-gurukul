using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetRecentSearches;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetRecentSearchesQueryHandlerTests
{
    private readonly Mock<IRecentSearchRepository> _repositoryMock = TestMocks.CreateRecentSearchRepository();
    private readonly Mock<ILogger<GetRecentSearchesQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetRecentSearchesQueryHandler>();
    private readonly GetRecentSearchesQueryHandler _handler;

    public GetRecentSearchesQueryHandlerTests()
    {
        _handler = new GetRecentSearchesQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsRecentSearches()
    {
        var userId = Guid.NewGuid();
        var searches = new List<RecentSearch>
        {
            new() { Id = Guid.NewGuid(), QueryText = "cricket", FiltersJson = "{}", ResultCount = 10, SearchedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searches);

        var result = await _handler.Handle(new GetRecentSearchesQuery { UserId = userId, Limit = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().QueryText.Should().Be("cricket");
    }
}
