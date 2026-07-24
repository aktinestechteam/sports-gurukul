using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetSavedCoachSearches;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetSavedCoachSearchesQueryHandlerTests
{
    private readonly Mock<ISavedSearchRepository> _repositoryMock = TestMocks.CreateSavedSearchRepository();
    private readonly Mock<ILogger<GetSavedCoachSearchesQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetSavedCoachSearchesQueryHandler>();
    private readonly GetSavedCoachSearchesQueryHandler _handler;

    public GetSavedCoachSearchesQueryHandlerTests()
    {
        _handler = new GetSavedCoachSearchesQueryHandler(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSavedSearches()
    {
        var userId = Guid.NewGuid();
        var searches = new List<SavedSearch>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, Name = "Search 1", FiltersJson = "{}", UsageCount = 5, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), UserId = userId, Name = "Search 2", FiltersJson = "{\"city\":\"Pune\"}", UsageCount = 2, CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searches);

        var result = await _handler.Handle(new GetSavedCoachSearchesQuery { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Name.Should().Be("Search 1");
        result.Value[0].UsageCount.Should().Be(5);
    }

    [Fact]
    public async Task Handle_NoSearches_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SavedSearch>());

        var result = await _handler.Handle(new GetSavedCoachSearchesQuery { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
