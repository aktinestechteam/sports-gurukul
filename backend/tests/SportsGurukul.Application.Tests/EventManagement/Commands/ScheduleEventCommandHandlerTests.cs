using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.ScheduleEvent;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class ScheduleEventCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventLifecycleService> _lifecycleService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<ScheduleEventCommandHandler>> _logger;
    private readonly ScheduleEventCommandHandler _handler;

    public ScheduleEventCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _lifecycleService = EventMockFactory.CreateLifecycleService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<ScheduleEventCommandHandler>();
        _handler = new ScheduleEventCommandHandler(_eventRepo.Object, _lifecycleService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new ScheduleEventCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_EndDateBeforeStartDate_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new ScheduleEventCommand
        {
            EventId = evt.Id,
            StartDate = DateTime.UtcNow.AddDays(17),
            EndDate = DateTime.UtcNow.AddDays(10),
            RegistrationOpenDate = DateTime.UtcNow,
            RegistrationCloseDate = DateTime.UtcNow.AddDays(5)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("end date must be after start date");
    }

    [Fact]
    public async Task Handle_RegCloseAfterStartDate_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new ScheduleEventCommand
        {
            EventId = evt.Id,
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(17),
            RegistrationOpenDate = DateTime.UtcNow,
            RegistrationCloseDate = DateTime.UtcNow.AddDays(15)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Registration close date must be before event start date");
    }

    [Fact]
    public async Task Handle_ValidSchedule_TransitionsToScheduled()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _lifecycleService.Setup(x => x.ValidateStateTransitionAsync(EventStatus.Draft, EventStatus.Scheduled, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EventStatus.Scheduled);

        var result = await _handler.Handle(new ScheduleEventCommand
        {
            EventId = evt.Id,
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(37),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        evt.Status.Should().Be(EventStatus.Scheduled);
    }
}
