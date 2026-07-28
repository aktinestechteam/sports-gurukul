using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Services;

public class EventRegistrationServiceTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventRegistrationRepository> _regRepo;
    private readonly Mock<ILogger<EventRegistrationService>> _logger;
    private readonly EventRegistrationService _service;

    public EventRegistrationServiceTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _regRepo = EventMockFactory.CreateRegistrationRepository();
        _logger = EventMockFactory.CreateLogger<EventRegistrationService>();
        _service = new EventRegistrationService(_eventRepo.Object, _regRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task GenerateRegistrationNumberAsync_ReturnsFormattedNumber()
    {
        _regRepo.Setup(x => x.CountSearchAsync(null, null, null, It.IsAny<CancellationToken>())).ReturnsAsync(10);

        var number = await _service.GenerateRegistrationNumberAsync();

        number.Should().StartWith("REG-");
        number.Should().Contain(DateTime.UtcNow.ToString("yyyyMMdd"));
    }

    [Fact]
    public async Task IsRegistrationAllowedAsync_RegistrationOpen_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        var result = await _service.IsRegistrationAllowedAsync(evt);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsRegistrationAllowedAsync_DraftStatus_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        var result = await _service.IsRegistrationAllowedAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCapacityAvailableAsync_NoMaxParticipants_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.MaxParticipants = null;
        var result = await _service.IsCapacityAvailableAsync(evt);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCapacityAvailableAsync_WithCapacity_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.MaxParticipants = 50;
        _regRepo.Setup(x => x.GetRegistrationCountAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(10);
        var result = await _service.IsCapacityAvailableAsync(evt);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCapacityAvailableAsync_Full_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.MaxParticipants = 50;
        _regRepo.Setup(x => x.GetRegistrationCountAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(50);
        var result = await _service.IsCapacityAvailableAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DetermineInitialStatusAsync_FreeEvent_ReturnsApproved()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.RegistrationType = EventRegistrationType.Free;
        var result = await _service.DetermineInitialStatusAsync(evt);
        result.Should().Be(EventRegistrationStatus.Approved);
    }

    [Fact]
    public async Task DetermineInitialStatusAsync_ApprovalRequired_ReturnsPending()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.RegistrationType = EventRegistrationType.ApprovalRequired;
        var result = await _service.DetermineInitialStatusAsync(evt);
        result.Should().Be(EventRegistrationStatus.Pending);
    }

    [Fact]
    public async Task DetermineInitialStatusAsync_Waitlist_ReturnsWaitlisted()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.RegistrationType = EventRegistrationType.Waitlist;
        var result = await _service.DetermineInitialStatusAsync(evt);
        result.Should().Be(EventRegistrationStatus.Waitlisted);
    }

    [Fact]
    public async Task ProcessWaitlistPromotionAsync_HasWaitlisted_PromotesFirst()
    {
        var eventId = Guid.NewGuid();
        var reg = EventDataFixture.CreateWaitlistedRegistration(eventId: eventId);
        _regRepo.Setup(x => x.GetByEventIdWithStatusAsync(eventId, EventRegistrationStatus.Waitlisted, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventRegistration> { reg });

        var result = await _service.ProcessWaitlistPromotionAsync(eventId);

        result.Should().NotBeNull();
        result!.Status.Should().Be(EventRegistrationStatus.Approved);
    }
}
