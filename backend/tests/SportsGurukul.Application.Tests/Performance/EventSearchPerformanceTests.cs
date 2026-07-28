using System.Diagnostics;
using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.Autocomplete;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.SearchEvents;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Performance;

public class EventSearchPerformanceTests
{
    private readonly Mock<IEventSearchRepository> _searchRepositoryMock = new();
    private readonly Mock<ICacheService> _cacheServiceMock = new();
    private readonly Mock<ILogger<SearchEventsQueryHandler>> _searchLoggerMock = new();
    private readonly Mock<ILogger<AutocompleteQueryHandler>> _autocompleteLoggerMock = new();

    private readonly SearchEventsQueryHandler _searchHandler;
    private readonly AutocompleteQueryHandler _autocompleteHandler;

    public EventSearchPerformanceTests()
    {
        _searchHandler = new SearchEventsQueryHandler(_searchRepositoryMock.Object, _searchLoggerMock.Object);
        _autocompleteHandler = new AutocompleteQueryHandler(_searchRepositoryMock.Object, _cacheServiceMock.Object, _autocompleteLoggerMock.Object);
    }

    [Fact]
    public async Task SearchEvents_CompletesWithin200Ms_Target()
    {
        var events = GenerateEvents(20);

        _searchRepositoryMock.Setup(r => r.SearchEventsAsync(
            It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(),
            It.IsAny<decimal?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        _searchRepositoryMock.Setup(r => r.CountSearchEventsAsync(
            It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(),
            It.IsAny<decimal?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        var stopwatch = Stopwatch.StartNew();

        var result = await _searchHandler.Handle(new SearchEventsQuery
        {
            SearchTerm = "cricket",
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        stopwatch.Stop();

        result.IsSuccess.Should().BeTrue();
        result.Value!.SearchTimeMs.Should().BeLessThan(200);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(200,
            "Search operation should complete within 200ms target");
    }

    [Fact]
    public async Task Autocomplete_CompletesWithin50Ms_Target()
    {
        _cacheServiceMock.Setup(r => r.GetAsync<List<Application.Features.EventSearchDiscovery.DTOs.EventAutocompleteSuggestionDto>>(
            It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Application.Features.EventSearchDiscovery.DTOs.EventAutocompleteSuggestionDto>?)null);

        var suggestions = new List<EventAutocompleteResult>
        {
            new() { Id = Guid.NewGuid(), Text = "Cricket Championship", Type = "Event" }
        };

        _searchRepositoryMock.Setup(r => r.GetAutocompleteSuggestionsAsync(
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(suggestions);

        var stopwatch = Stopwatch.StartNew();

        var result = await _autocompleteHandler.Handle(new AutocompleteQuery
        {
            Prefix = "Cricket",
            Limit = 10
        }, CancellationToken.None);

        stopwatch.Stop();

        result.IsSuccess.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(50,
            "Autocomplete should complete within 50ms target");
    }

    [Fact]
    public async Task SearchEvents_LargeDataset_CompletesWithinTarget()
    {
        var events = GenerateEvents(100);

        _searchRepositoryMock.Setup(r => r.SearchEventsAsync(
            It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(),
            It.IsAny<decimal?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        _searchRepositoryMock.Setup(r => r.CountSearchEventsAsync(
            It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(),
            It.IsAny<decimal?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(500);

        var stopwatch = Stopwatch.StartNew();

        var result = await _searchHandler.Handle(new SearchEventsQuery
        {
            SearchTerm = "tournament",
            SportId = Guid.NewGuid(),
            City = "Mumbai",
            SortBy = "Popularity",
            Page = 1,
            PageSize = 100
        }, CancellationToken.None);

        stopwatch.Stop();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(100);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(200,
            "Large dataset search should still complete within 200ms target");
    }

    private static List<Event> GenerateEvents(int count)
    {
        return Enumerable.Range(1, count).Select(i => new Event
        {
            Id = Guid.NewGuid(),
            EventName = $"Event {i}",
            EventCode = $"EVT-{i:D4}",
            Description = $"Description for event {i}",
            ShortDescription = $"Short desc {i}",
            Status = EventStatus.RegistrationOpen,
            StartDate = DateTime.UtcNow.AddDays(i),
            EndDate = DateTime.UtcNow.AddDays(i + 1),
            RegistrationFee = i * 100,
            IsFeatured = i % 5 == 0,
            IsPublic = true,
            Sport = new Sport { Id = Guid.NewGuid(), Name = "Cricket", Code = "CRK", OlympicSport = true, SportCategoryId = Guid.NewGuid() },
            EventType = new EventTypeEntity { Id = Guid.NewGuid(), Name = "Competition", Code = "COMPETITION" },
            Venues = new List<EventVenue>
            {
                new() { Id = Guid.NewGuid(), VenueName = $"Venue {i}", City = "Mumbai", Latitude = 19.0760m + i * 0.01m, Longitude = 72.8777m, IsPrimary = true }
            },
            Registrations = new List<EventRegistration>(),
            Feedbacks = new List<EventFeedback>()
        }).ToList();
    }
}
