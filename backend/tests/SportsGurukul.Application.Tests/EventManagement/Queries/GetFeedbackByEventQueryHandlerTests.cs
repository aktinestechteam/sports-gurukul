using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.GetFeedbackByEvent;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class GetFeedbackByEventQueryHandlerTests
{
    private readonly Mock<IEventFeedbackRepository> _feedbackRepo;
    private readonly Mock<ILogger<GetFeedbackByEventQueryHandler>> _logger;
    private readonly GetFeedbackByEventQueryHandler _handler;

    public GetFeedbackByEventQueryHandlerTests()
    {
        _feedbackRepo = EventMockFactory.CreateFeedbackRepository();
        _logger = EventMockFactory.CreateLogger<GetFeedbackByEventQueryHandler>();
        _handler = new GetFeedbackByEventQueryHandler(_feedbackRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFeedbackList()
    {
        var eventId = Guid.NewGuid();
        _feedbackRepo.Setup(x => x.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventFeedback>());

        var result = await _handler.Handle(new GetFeedbackByEventQuery { EventId = eventId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithFeedback_ReturnsMappedDtos()
    {
        var eventId = Guid.NewGuid();
        var feedbacks = new List<Domain.Entities.EventFeedback>
        {
            EventDataFixture.CreateFeedback(eventId: eventId)
        };
        _feedbackRepo.Setup(x => x.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(feedbacks);

        var result = await _handler.Handle(new GetFeedbackByEventQuery { EventId = eventId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }
}
