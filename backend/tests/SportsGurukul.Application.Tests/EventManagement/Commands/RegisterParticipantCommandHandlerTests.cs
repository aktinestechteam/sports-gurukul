using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.RegisterParticipant;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class RegisterParticipantCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventRegistrationRepository> _regRepo;
    private readonly Mock<IEventRegistrationService> _regService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<RegisterParticipantCommandHandler>> _logger;
    private readonly RegisterParticipantCommandHandler _handler;

    public RegisterParticipantCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _regRepo = EventMockFactory.CreateRegistrationRepository();
        _regService = EventMockFactory.CreateRegistrationService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<RegisterParticipantCommandHandler>();
        _handler = new RegisterParticipantCommandHandler(_eventRepo.Object, _regRepo.Object, _regService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new RegisterParticipantCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_RegistrationNotAllowed_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _regService.Setup(x => x.IsRegistrationAllowedAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new RegisterParticipantCommand { EventId = evt.Id, AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Registration is not allowed");
    }

    [Fact]
    public async Task Handle_NoCapacity_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _regService.Setup(x => x.IsRegistrationAllowedAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _regService.Setup(x => x.IsCapacityAvailableAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new RegisterParticipantCommand { EventId = evt.Id, AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("maximum capacity");
    }

    [Fact]
    public async Task Handle_DuplicateRegistration_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _regService.Setup(x => x.IsRegistrationAllowedAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _regService.Setup(x => x.IsCapacityAvailableAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _regService.Setup(x => x.IsDuplicateRegistrationAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _handler.Handle(new RegisterParticipantCommand { EventId = evt.Id, AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already registered");
    }

    [Fact]
    public async Task Handle_FreeEvent_RegistersWithApprovedStatus()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _regService.Setup(x => x.IsRegistrationAllowedAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _regService.Setup(x => x.IsCapacityAvailableAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _regService.Setup(x => x.IsDuplicateRegistrationAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _regService.Setup(x => x.DetermineInitialStatusAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(EventRegistrationStatus.Approved);
        _regService.Setup(x => x.GenerateRegistrationNumberAsync(It.IsAny<CancellationToken>())).ReturnsAsync("REG-001");

        var result = await _handler.Handle(new RegisterParticipantCommand { EventId = evt.Id, AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(EventRegistrationStatus.Approved.ToString());
    }

    [Fact]
    public async Task Handle_ValidRegistration_CallsAddAndSave()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _regService.Setup(x => x.IsRegistrationAllowedAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _regService.Setup(x => x.IsCapacityAvailableAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _regService.Setup(x => x.IsDuplicateRegistrationAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _regService.Setup(x => x.DetermineInitialStatusAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(EventRegistrationStatus.Approved);
        _regService.Setup(x => x.GenerateRegistrationNumberAsync(It.IsAny<CancellationToken>())).ReturnsAsync("REG-001");

        await _handler.Handle(new RegisterParticipantCommand { EventId = evt.Id, AthleteId = Guid.NewGuid() }, CancellationToken.None);

        _regRepo.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.EventRegistration>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
