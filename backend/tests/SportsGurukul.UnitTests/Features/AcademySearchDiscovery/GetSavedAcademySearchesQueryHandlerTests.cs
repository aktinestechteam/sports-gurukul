using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSavedAcademySearches;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class GetSavedAcademySearchesQueryHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<GetSavedAcademySearchesQueryHandler>> _loggerMock;
    private readonly GetSavedAcademySearchesQueryHandler _handler;

    public GetSavedAcademySearchesQueryHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<GetSavedAcademySearchesQueryHandler>>();
        _handler = new GetSavedAcademySearchesQueryHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserId_ReturnsSavedSearches()
    {
        var userId = Guid.NewGuid();
        var savedSearches = new List<SavedAcademySearch>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SearchName = "Cricket in Mumbai",
                SearchTerm = "cricket",
                City = "Mumbai",
                ResultCount = 15
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SearchName = "Football in Delhi",
                SearchTerm = "football",
                City = "Delhi",
                ResultCount = 8
            }
        };

        _academySearchRepositoryMock
            .Setup(r => r.GetSavedSearchesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedSearches);

        var result = await _handler.Handle(
            new GetSavedAcademySearchesQuery { UserId = userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value!.First().SearchName.Should().Be("Cricket in Mumbai");
        result.Value!.Last().SearchName.Should().Be("Football in Delhi");
    }
}
