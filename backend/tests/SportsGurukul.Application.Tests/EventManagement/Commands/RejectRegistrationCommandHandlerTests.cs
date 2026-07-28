using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.RejectRegistration;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class RejectRegistrationCommandHandlerTests
{
    private readonly Mock<IEventRegistrationRepository> _regRepo;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<RejectRegistrationCommandHandler>> _logger;
    private readonly RejectRegistrationCommandHandler _handler;

    public RejectRegistrationCommandHandlerTests()
    {
        _regRepo = EventMockFactory.CreateRegistrationRepository();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<RejectRegistrationCommandHandler>();
        _handler = new RejectRegistrationCommandHandler(_regRepo.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_RegistrationNotFound_ReturnsFailure()
    {
        _regRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.EventRegistration?)null);

        var result = await _handler.Handle(new RejectRegistrationCommand { RegistrationId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Registration not found");
    }

    [Fact]
    public async Task Handle_PendingRegistration_Rejected()
    {
        var reg = EventDataFixture.CreatePendingRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new RejectRegistrationCommand { RegistrationId = reg.Id, Reason = "Not qualified" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        reg.Status.Should().Be(EventRegistrationStatus.Rejected);
        reg.RejectionReason.Should().Be("Not qualified");
    }

    [Fact]
    public async Task Handle_WaitlistedRegistration_Rejected()
    {
        var reg = EventDataFixture.CreateWaitlistedRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new RejectRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        reg.Status.Should().Be(EventRegistrationStatus.Rejected);
    }

    [Fact]
    public async Task Handle_AlreadyApproved_ReturnsFailure()
    {
        var reg = EventDataFixture.CreateApprovedRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new RejectRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only pending or waitlisted");
    }

    [Fact]
    public async Task Handle_AlreadyCancelled_ReturnsFailure()
    {
        var reg = EventDataFixture.CreateCancelledRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new RejectRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only pending or waitlisted");
    }
}
