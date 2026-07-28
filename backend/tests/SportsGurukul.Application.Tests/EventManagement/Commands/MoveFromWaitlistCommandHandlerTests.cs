using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.MoveFromWaitlist;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class MoveFromWaitlistCommandHandlerTests
{
    private readonly Mock<IEventRegistrationRepository> _regRepo;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<MoveFromWaitlistCommandHandler>> _logger;
    private readonly MoveFromWaitlistCommandHandler _handler;

    public MoveFromWaitlistCommandHandlerTests()
    {
        _regRepo = EventMockFactory.CreateRegistrationRepository();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<MoveFromWaitlistCommandHandler>();
        _handler = new MoveFromWaitlistCommandHandler(_regRepo.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_RegistrationNotFound_ReturnsFailure()
    {
        _regRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.EventRegistration?)null);

        var result = await _handler.Handle(new MoveFromWaitlistCommand { RegistrationId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Registration not found");
    }

    [Fact]
    public async Task Handle_WaitlistedRegistration_Moved()
    {
        var reg = EventDataFixture.CreateWaitlistedRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new MoveFromWaitlistCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        reg.Status.Should().Be(EventRegistrationStatus.Approved);
        reg.ApprovalDate.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_NonWaitlisted_ReturnsFailure()
    {
        var reg = EventDataFixture.CreatePendingRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new MoveFromWaitlistCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only waitlisted");
    }
}
