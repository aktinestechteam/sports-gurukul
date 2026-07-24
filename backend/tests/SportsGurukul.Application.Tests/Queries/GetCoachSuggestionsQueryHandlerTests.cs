using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachSuggestions;
using SportsGurukul.Application.Tests.Common;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachSuggestionsQueryHandlerTests
{
    private readonly Mock<ICoachSearchRepository> _searchRepoMock = TestMocks.CreateCoachSearchRepository();
    private readonly Mock<ICacheService> _cacheMock = TestMocks.CreateCacheService();
    private readonly Mock<ILogger<GetCoachSuggestionsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachSuggestionsQueryHandler>();
    private readonly GetCoachSuggestionsQueryHandler _handler;

    public GetCoachSuggestionsQueryHandlerTests()
    {
        _handler = new GetCoachSuggestionsQueryHandler(
            _searchRepoMock.Object,
            _cacheMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedResults()
    {
        var cached = new List<CoachSearchSuggestionDto>
        {
            new() { Text = "Rahul Sharma", Type = "Name", SubText = "Coach Name" }
        };
        _cacheMock.Setup(c => c.GetAsync<List<CoachSearchSuggestionDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var result = await _handler.Handle(new GetCoachSuggestionsQuery { Prefix = "Rah", Limit = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        _searchRepoMock.Verify(r => r.GetSearchSuggestionsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CacheMiss_FetchesFromRepositoryAndCaches()
    {
        _cacheMock.Setup(c => c.GetAsync<List<CoachSearchSuggestionDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<CoachSearchSuggestionDto>?)null);
        _searchRepoMock.Setup(r => r.GetSearchSuggestionsAsync("Rah", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "Rahul Sharma", "COACH-20250101-001" });

        var result = await _handler.Handle(new GetCoachSuggestionsQuery { Prefix = "Rah", Limit = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Type.Should().Be("Name");
        result.Value[1].Type.Should().Be("CoachCode");
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyPrefix_ReturnsEmptyList()
    {
        _cacheMock.Setup(c => c.GetAsync<List<CoachSearchSuggestionDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<CoachSearchSuggestionDto>?)null);
        _searchRepoMock.Setup(r => r.GetSearchSuggestionsAsync("", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        var result = await _handler.Handle(new GetCoachSuggestionsQuery { Prefix = "", Limit = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
