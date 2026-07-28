using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.CancelEvent;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class CancelEventCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventLifecycleService> _lifecycleService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<CancelEventCommandHandler>> _logger;
    private readonly CancelEventCommandHandler _handler;

    public CancelEventCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _lifecycleService = EventMockFactory.CreateLifecycleService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<CancelEventCommandHandler>();
        _handler = new CancelEventCommandHandler(_eventRepo.Object, _lifecycleService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new CancelEventCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_CannotCancel_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateInProgressEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.CanCancelAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new CancelEventCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("cannot be cancelled");
    }

    [Fact]
    public async Task Handle_ValidCancel_TransitionsToCancelled()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.CanCancelAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _lifecycleService.Setup(x => x.ValidateStateTransitionAsync(EventStatus.Draft, EventStatus.Cancelled, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventStatus.Cancelled);

        var result = await _handler.Handle(new CancelEventCommand { EventId = evt.Id, Reason = "Too expensive" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        evt.Status.Should().Be(EventStatus.Cancelled);
        evt.CancellationPolicy.Should().Be("Too expensive");
    }

    [Fact]
    public async Task Handle_CancelWithoutReason_KeepsExistingPolicy()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        evt.CancellationPolicy = "Existing policy";
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.CanCancelAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _lifecycleService.Setup(x => x.ValidateStateTransitionAsync(EventStatus.Draft, EventStatus.Cancelled, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventStatus.Cancelled);

        var result = await _handler.Handle(new CancelEventCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        evt.CancellationPolicy.Should().Be("Existing policy");
    }
}
