using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Services;

public class EventLifecycleServiceTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<ILogger<EventLifecycleService>> _logger;
    private readonly EventLifecycleService _service;

    public EventLifecycleServiceTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _logger = EventMockFactory.CreateLogger<EventLifecycleService>();
        _service = new EventLifecycleService(_eventRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task GenerateEventCodeAsync_ReturnsFormattedCode()
    {
        _eventRepo.Setup(x => x.CountSearchAsync(null, null, null, null, null, It.IsAny<CancellationToken>())).ReturnsAsync(5);

        var code = await _service.GenerateEventCodeAsync();

        code.Should().StartWith("EVT-");
        code.Should().Contain(DateTime.UtcNow.ToString("yyyyMMdd"));
    }

    [Theory]
    [InlineData(EventStatus.Draft, EventStatus.Published, true)]
    [InlineData(EventStatus.Draft, EventStatus.Cancelled, true)]
    [InlineData(EventStatus.Draft, EventStatus.Archived, true)]
    [InlineData(EventStatus.Published, EventStatus.RegistrationOpen, true)]
    [InlineData(EventStatus.Published, EventStatus.Scheduled, true)]
    [InlineData(EventStatus.InProgress, EventStatus.Completed, true)]
    [InlineData(EventStatus.Archived, EventStatus.Completed, false)]
    [InlineData(EventStatus.Draft, EventStatus.InProgress, false)]
    public async Task ValidateStateTransitionAsync_ReturnsTargetForValidTransitions(EventStatus current, EventStatus target, bool expectedValid)
    {
        if (expectedValid)
        {
            var result = await _service.ValidateStateTransitionAsync(current, target);
            result.Should().Be(target);
        }
        else
        {
            var act = () => _service.ValidateStateTransitionAsync(current, target);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }

    [Fact]
    public async Task CanPublishAsync_DraftWithValidData_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        var result = await _service.CanPublishAsync(evt);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanPublishAsync_NotDraft_ReturnsFalse()
    {
        var evt = EventDataFixture.CreatePublishedEvent();
        var result = await _service.CanPublishAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCompleteAsync_InProgressEvent_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateInProgressEvent();
        evt.EndDate = DateTime.UtcNow.AddDays(-1);
        var result = await _service.CanCompleteAsync(evt);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanCompleteAsync_DraftEvent_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        var result = await _service.CanCompleteAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanArchiveAsync_CompletedEvent_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        var result = await _service.CanArchiveAsync(evt);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanArchiveAsync_DraftEvent_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        var result = await _service.CanArchiveAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCancelAsync_DraftEvent_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        var result = await _service.CanCancelAsync(evt);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanCancelAsync_InProgressEvent_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateInProgressEvent();
        var result = await _service.CanCancelAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCancelAsync_CompletedEvent_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        var result = await _service.CanCancelAsync(evt);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanCancelAsync_ArchivedEvent_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateArchivedEvent();
        var result = await _service.CanCancelAsync(evt);
        result.Should().BeFalse();
    }
}
