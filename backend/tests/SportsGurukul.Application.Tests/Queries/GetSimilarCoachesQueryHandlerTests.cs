using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetSimilarCoaches;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class GetSimilarCoachesQueryHandlerTests
{
    private readonly Mock<ICoachSearchRepository> _searchRepoMock = TestMocks.CreateCoachSearchRepository();
    private readonly Mock<ICacheService> _cacheMock = TestMocks.CreateCacheService();
    private readonly Mock<ILogger<GetSimilarCoachesQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetSimilarCoachesQueryHandler>();
    private readonly GetSimilarCoachesQueryHandler _handler;

    public GetSimilarCoachesQueryHandlerTests()
    {
        _handler = new GetSimilarCoachesQueryHandler(
            _searchRepoMock.Object,
            _cacheMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedResults()
    {
        var cached = new List<SimilarCoachDto>
        {
            new() { Id = Guid.NewGuid(), FullName = "Priya Patel", MatchScore = 3 }
        };
        _cacheMock.Setup(c => c.GetAsync<List<SimilarCoachDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var result = await _handler.Handle(new GetSimilarCoachesQuery { CoachId = Guid.NewGuid(), Limit = 5 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        _searchRepoMock.Verify(r => r.GetSimilarCoachesAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CacheMiss_FetchesAndMapsCoaches()
    {
        var coachId = Guid.NewGuid();
        _cacheMock.Setup(c => c.GetAsync<List<SimilarCoachDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<SimilarCoachDto>?)null);

        var sportId = Guid.NewGuid();
        var similarCoaches = new List<Coach>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CoachCode = "COACH-001",
                CoachingLevel = CoachingLevel.Senior,
                YearsOfExperience = 8,
                VerificationStatus = VerificationStatus.Verified,
                User = new User { FullName = "Test Coach" },
                CoachSports = new List<CoachSport>
                {
                    new() { SportId = sportId, IsPrimarySport = true, Sport = new Sport { Name = "Cricket" } }
                },
                Location = new CoachLocation { City = "Mumbai", State = "Maharashtra" }
            }
        };
        _searchRepoMock.Setup(r => r.GetSimilarCoachesAsync(coachId, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(similarCoaches);

        var result = await _handler.Handle(new GetSimilarCoachesQuery { CoachId = coachId, Limit = 5 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].FullName.Should().Be("Test Coach");
        result.Value[0].CoachingLevel.Should().Be("Senior");
        result.Value[0].IsVerified.Should().BeTrue();
        result.Value[0].PrimarySport.Should().Be("Cricket");
        result.Value[0].City.Should().Be("Mumbai");
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoSimilarCoaches_ReturnsEmptyList()
    {
        var coachId = Guid.NewGuid();
        _cacheMock.Setup(c => c.GetAsync<List<SimilarCoachDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<SimilarCoachDto>?)null);
        _searchRepoMock.Setup(r => r.GetSimilarCoachesAsync(coachId, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Coach>());

        var result = await _handler.Handle(new GetSimilarCoachesQuery { CoachId = coachId, Limit = 5 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
