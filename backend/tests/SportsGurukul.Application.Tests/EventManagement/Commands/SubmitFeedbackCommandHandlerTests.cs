using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.SubmitFeedback;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class SubmitFeedbackCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventFeedbackService> _feedbackService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<SubmitFeedbackCommandHandler>> _logger;
    private readonly SubmitFeedbackCommandHandler _handler;

    public SubmitFeedbackCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _feedbackService = EventMockFactory.CreateFeedbackService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<SubmitFeedbackCommandHandler>();
        _handler = new SubmitFeedbackCommandHandler(_eventRepo.Object, _feedbackService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new SubmitFeedbackCommand { EventId = Guid.NewGuid(), UserId = Guid.NewGuid(), OverallRating = 4 }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_FeedbackNotAllowed_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _feedbackService.Setup(x => x.CanSubmitFeedbackAsync(evt, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new SubmitFeedbackCommand { EventId = evt.Id, UserId = Guid.NewGuid(), OverallRating = 4 }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("cannot be submitted");
    }

    [Fact]
    public async Task Handle_ValidFeedback_Created()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _feedbackService.Setup(x => x.CanSubmitFeedbackAsync(evt, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var userId = Guid.NewGuid();
        var result = await _handler.Handle(new SubmitFeedbackCommand
        {
            EventId = evt.Id,
            UserId = userId,
            OverallRating = 4,
            ContentRating = 5,
            Comments = "Great event!",
            WouldRecommend = true
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.OverallRating.Should().Be("Good");
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
