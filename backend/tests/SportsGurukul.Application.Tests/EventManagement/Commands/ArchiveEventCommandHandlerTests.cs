using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.ArchiveEvent;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class ArchiveEventCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventLifecycleService> _lifecycleService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<ArchiveEventCommandHandler>> _logger;
    private readonly ArchiveEventCommandHandler _handler;

    public ArchiveEventCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _lifecycleService = EventMockFactory.CreateLifecycleService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<ArchiveEventCommandHandler>();
        _handler = new ArchiveEventCommandHandler(_eventRepo.Object, _lifecycleService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new ArchiveEventCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_CannotArchive_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.CanArchiveAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new ArchiveEventCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("can only be archived");
    }

    [Fact]
    public async Task Handle_CompletedEvent_CanArchive()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.CanArchiveAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _lifecycleService.Setup(x => x.ValidateStateTransitionAsync(EventStatus.Completed, EventStatus.Archived, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventStatus.Archived);

        var result = await _handler.Handle(new ArchiveEventCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        evt.Status.Should().Be(EventStatus.Archived);
    }

    [Fact]
    public async Task Handle_CancelledEvent_CanArchive()
    {
        var evt = EventDataFixture.CreateCancelledEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.CanArchiveAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _lifecycleService.Setup(x => x.ValidateStateTransitionAsync(EventStatus.Cancelled, EventStatus.Archived, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventStatus.Archived);

        var result = await _handler.Handle(new ArchiveEventCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        evt.Status.Should().Be(EventStatus.Archived);
    }
}
