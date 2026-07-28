using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.Autocomplete;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class AutocompleteTests
{
    private readonly Mock<IEventSearchRepository> _searchRepositoryMock = new();
    private readonly Mock<ICacheService> _cacheServiceMock = new();
    private readonly Mock<ILogger<AutocompleteQueryHandler>> _loggerMock = new();
    private readonly AutocompleteQueryHandler _handler;

    public AutocompleteTests()
    {
        _handler = new AutocompleteQueryHandler(_searchRepositoryMock.Object, _cacheServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShortPrefix_ReturnsEmpty()
    {
        var result = await _handler.Handle(new AutocompleteQuery { Prefix = "A", Limit = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_EmptyPrefix_ReturnsEmpty()
    {
        var result = await _handler.Handle(new AutocompleteQuery { Prefix = "", Limit = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CachedResult_ReturnsCached()
    {
        var cached = new List<EventAutocompleteSuggestionDto>
        {
            new() { Id = Guid.NewGuid(), Text = "Cricket Championship", Type = "Event" }
        };
        _cacheServiceMock.Setup(r => r.GetAsync<List<EventAutocompleteSuggestionDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var result = await _handler.Handle(new AutocompleteQuery { Prefix = "Cricket", Limit = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Count.Should().Be(1);
        result.Value[0].Text.Should().Be("Cricket Championship");
    }

    [Fact]
    public async Task Handle_ValidPrefix_FetchesFromRepository()
    {
        _cacheServiceMock.Setup(r => r.GetAsync<List<EventAutocompleteSuggestionDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<EventAutocompleteSuggestionDto>?)null);

        var autocompleteResults = new List<EventAutocompleteResult>
        {
            new() { Id = Guid.NewGuid(), Text = "Cricket Championship", Type = "Event", SubText = "Mumbai" }
        };
        _searchRepositoryMock.Setup(r => r.GetAutocompleteSuggestionsAsync("Cricket", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(autocompleteResults);

        var result = await _handler.Handle(new AutocompleteQuery { Prefix = "Cricket", Limit = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Count.Should().Be(1);
    }
}
