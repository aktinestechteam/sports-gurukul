using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.SearchEvents;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services.EventSearchDiscovery;

public class SearchEventsQueryTests
{
    private readonly Mock<IEventSearchRepository> _searchRepositoryMock = new();
    private readonly Mock<ILogger<SearchEventsQueryHandler>> _loggerMock = new();
    private readonly SearchEventsQueryHandler _handler;

    public SearchEventsQueryTests()
    {
        _handler = new SearchEventsQueryHandler(_searchRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_EmptySearch_ReturnsEmptyResults()
    {
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
            .ReturnsAsync(new List<Event>());

        _searchRepositoryMock.Setup(r => r.CountSearchEventsAsync(
            It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(),
            It.IsAny<decimal?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(new SearchEventsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithEvents_ReturnsMappedResults()
    {
        var eventId = Guid.NewGuid();
        var events = new List<Event>
        {
            new()
            {
                Id = eventId,
                EventName = "Cricket Tournament",
                EventCode = "EVT-001",
                Status = EventStatus.RegistrationOpen,
                StartDate = DateTime.UtcNow.AddDays(10),
                RegistrationFee = 500,
                IsFeatured = true,
                IsPublic = true,
                Sport = new Sport { Name = "Cricket" },
                Venues = new List<EventVenue>
                {
                    new() { City = "Mumbai", State = "Maharashtra", IsPrimary = true }
                },
                Registrations = new List<EventRegistration>(),
                Feedbacks = new List<EventFeedback>()
            }
        };

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
            .ReturnsAsync(1);

        var result = await _handler.Handle(new SearchEventsQuery
        {
            SearchTerm = "cricket",
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items[0].EventName.Should().Be("Cricket Tournament");
        result.Value.Items[0].SportName.Should().Be("Cricket");
        result.Value.Items[0].City.Should().Be("Mumbai");
        result.Value.SearchTimeMs.Should().BeGreaterThan(0);
    }
}
