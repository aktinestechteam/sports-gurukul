using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Services;

public class EventAnnouncementServiceTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<ILogger<EventAnnouncementService>> _logger;
    private readonly EventAnnouncementService _service;

    public EventAnnouncementServiceTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _logger = EventMockFactory.CreateLogger<EventAnnouncementService>();
        _service = new EventAnnouncementService(_eventRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task CanPublishAnnouncementAsync_DraftEvent_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        var result = await _service.CanPublishAnnouncementAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanPublishAnnouncementAsync_ArchivedEvent_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateArchivedEvent();
        var result = await _service.CanPublishAnnouncementAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanPublishAnnouncementAsync_PublishedEvent_ReturnsTrue()
    {
        var evt = EventDataFixture.CreatePublishedEvent();
        var result = await _service.CanPublishAnnouncementAsync(evt);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanPublishAnnouncementAsync_RegistrationOpen_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        var result = await _service.CanPublishAnnouncementAsync(evt);
        result.Should().BeTrue();
    }
}
