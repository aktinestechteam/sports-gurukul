using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Services;

public class EventFeedbackServiceTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventFeedbackRepository> _feedbackRepo;
    private readonly Mock<ILogger<EventFeedbackService>> _logger;
    private readonly EventFeedbackService _service;

    public EventFeedbackServiceTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _feedbackRepo = EventMockFactory.CreateFeedbackRepository();
        _logger = EventMockFactory.CreateLogger<EventFeedbackService>();
        _service = new EventFeedbackService(_eventRepo.Object, _feedbackRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task CanSubmitFeedbackAsync_EventNotCompleted_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        var result = await _service.CanSubmitFeedbackAsync(evt, Guid.NewGuid());
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanSubmitFeedbackAsync_CompletedAndNoExisting_ReturnsTrue()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        var userId = Guid.NewGuid();
        _feedbackRepo.Setup(x => x.GetByEventAndUserAsync(evt.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.EventFeedback?)null);

        var result = await _service.CanSubmitFeedbackAsync(evt, userId);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanSubmitFeedbackAsync_AlreadySubmitted_ReturnsFalse()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        var userId = Guid.NewGuid();
        _feedbackRepo.Setup(x => x.GetByEventAndUserAsync(evt.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventDataFixture.CreateFeedback(eventId: evt.Id));

        var result = await _service.CanSubmitFeedbackAsync(evt, userId);
        result.Should().BeFalse();
    }
}
