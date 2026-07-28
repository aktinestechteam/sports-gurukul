using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.CloseRegistration;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class CloseRegistrationCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventLifecycleService> _lifecycleService;
    private readonly Mock<ILogger<CloseRegistrationCommandHandler>> _logger;
    private readonly CloseRegistrationCommandHandler _handler;

    public CloseRegistrationCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _lifecycleService = EventMockFactory.CreateLifecycleService();
        _logger = EventMockFactory.CreateLogger<CloseRegistrationCommandHandler>();
        _handler = new CloseRegistrationCommandHandler(_eventRepo.Object, _lifecycleService.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new CloseRegistrationCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_ValidClose_TransitionsToRegistrationClosed()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.ValidateStateTransitionAsync(EventStatus.RegistrationOpen, EventStatus.RegistrationClosed, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventStatus.RegistrationClosed);

        var result = await _handler.Handle(new CloseRegistrationCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        evt.Status.Should().Be(EventStatus.RegistrationClosed);
    }

    [Fact]
    public async Task Handle_InvalidTransition_ThrowsInvalidOperationException()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.ValidateStateTransitionAsync(EventStatus.Draft, EventStatus.RegistrationClosed, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cannot transition from Draft to RegistrationClosed."));

        var act = () => _handler.Handle(new CloseRegistrationCommand { EventId = evt.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
