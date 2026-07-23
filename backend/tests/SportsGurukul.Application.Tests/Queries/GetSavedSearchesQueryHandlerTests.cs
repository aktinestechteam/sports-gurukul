using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetSavedSearches;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetSavedSearchesQueryHandlerTests
{
    private readonly Mock<ISavedSearchRepository> _repositoryMock = TestMocks.CreateSavedSearchRepository();
    private readonly Mock<ILogger<GetSavedSearchesQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetSavedSearchesQueryHandler>();
    private readonly GetSavedSearchesQueryHandler _handler;

    public GetSavedSearchesQueryHandlerTests()
    {
        _handler = new GetSavedSearchesQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSavedSearches()
    {
        var userId = Guid.NewGuid();
        var searches = new List<SavedSearch>
        {
            new() { Id = Guid.NewGuid(), Name = "Test Search", FiltersJson = "{}", UsageCount = 5, CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searches);

        var result = await _handler.Handle(new GetSavedSearchesQuery { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Name.Should().Be("Test Search");
    }
}
