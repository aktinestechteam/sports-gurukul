using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSuggestions;
using SportsGurukul.Application.Tests.Common;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAthleteSuggestionsQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ICacheService> _cacheServiceMock = TestMocks.CreateCacheService();
    private readonly Mock<ILogger<GetAthleteSuggestionsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAthleteSuggestionsQueryHandler>();
    private readonly GetAthleteSuggestionsQueryHandler _handler;

    public GetAthleteSuggestionsQueryHandlerTests()
    {
        _handler = new GetAthleteSuggestionsQueryHandler(
            _athleteRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedResult()
    {
        var cached = new List<Features.AthleteManagement.DTOs.AthleteSearchSuggestionDto>
        {
            new() { Text = "Cricket", Type = "sport" }
        };
        _cacheServiceMock.Setup(c => c.GetAsync<List<Features.AthleteManagement.DTOs.AthleteSearchSuggestionDto>>(
            It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var result = await _handler.Handle(new GetAthleteSuggestionsQuery { Prefix = "cr", Limit = 5 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        _athleteRepositoryMock.Verify(r => r.GetSearchSuggestionsAsync(
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CacheMiss_FetchesFromRepository()
    {
        _cacheServiceMock.Setup(c => c.GetAsync<List<Features.AthleteManagement.DTOs.AthleteSearchSuggestionDto>>(
            It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Features.AthleteManagement.DTOs.AthleteSearchSuggestionDto>?)null);
        _athleteRepositoryMock.Setup(r => r.GetSearchSuggestionsAsync(
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Features.AthleteManagement.DTOs.AthleteSearchSuggestionDto>());

        var result = await _handler.Handle(new GetAthleteSuggestionsQuery { Prefix = "cr", Limit = 5 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _athleteRepositoryMock.Verify(r => r.GetSearchSuggestionsAsync("cr", 5, It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(c => c.SetAsync(
            It.IsAny<string>(), It.IsAny<List<Features.AthleteManagement.DTOs.AthleteSearchSuggestionDto>>(),
            It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
