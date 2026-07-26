using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetRecentAcademySearches;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class GetRecentAcademySearchesQueryHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<GetRecentAcademySearchesQueryHandler>> _loggerMock;
    private readonly GetRecentAcademySearchesQueryHandler _handler;

    public GetRecentAcademySearchesQueryHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<GetRecentAcademySearchesQueryHandler>>();
        _handler = new GetRecentAcademySearchesQueryHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserId_ReturnsRecentSearches()
    {
        var userId = Guid.NewGuid();
        var recentSearches = new List<RecentAcademySearch>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SearchTerm = "cricket academy",
                City = "Mumbai",
                AcademyCount = 10,
                SearchedAt = DateTime.UtcNow.AddMinutes(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SearchTerm = "football coaching",
                City = "Delhi",
                AcademyCount = 5,
                SearchedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        _academySearchRepositoryMock
            .Setup(r => r.GetRecentSearchesAsync(userId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recentSearches);

        var result = await _handler.Handle(
            new GetRecentAcademySearchesQuery { UserId = userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value!.First().SearchTerm.Should().Be("cricket academy");
        result.Value!.Last().SearchTerm.Should().Be("football coaching");
    }
}
