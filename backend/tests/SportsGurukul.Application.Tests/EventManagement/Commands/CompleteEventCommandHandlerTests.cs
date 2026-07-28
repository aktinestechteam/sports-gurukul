using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.CompleteEvent;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class CompleteEventCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventLifecycleService> _lifecycleService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<CompleteEventCommandHandler>> _logger;
    private readonly CompleteEventCommandHandler _handler;

    public CompleteEventCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _lifecycleService = EventMockFactory.CreateLifecycleService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<CompleteEventCommandHandler>();
        _handler = new CompleteEventCommandHandler(_eventRepo.Object, _lifecycleService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new CompleteEventCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_CannotComplete_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.CanCompleteAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new CompleteEventCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("can only be completed");
    }

    [Fact]
    public async Task Handle_ValidComplete_TransitionsToCompleted()
    {
        var evt = EventDataFixture.CreateInProgressEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.CanCompleteAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _lifecycleService.Setup(x => x.ValidateStateTransitionAsync(EventStatus.InProgress, EventStatus.Completed, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventStatus.Completed);

        var result = await _handler.Handle(new CompleteEventCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        evt.Status.Should().Be(EventStatus.Completed);
    }
}
