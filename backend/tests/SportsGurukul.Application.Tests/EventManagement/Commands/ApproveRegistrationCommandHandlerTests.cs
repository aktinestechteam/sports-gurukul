using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.ApproveRegistration;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class ApproveRegistrationCommandHandlerTests
{
    private readonly Mock<IEventRegistrationRepository> _regRepo;
    private readonly Mock<IEventRegistrationService> _regService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<ApproveRegistrationCommandHandler>> _logger;
    private readonly ApproveRegistrationCommandHandler _handler;

    public ApproveRegistrationCommandHandlerTests()
    {
        _regRepo = EventMockFactory.CreateRegistrationRepository();
        _regService = EventMockFactory.CreateRegistrationService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<ApproveRegistrationCommandHandler>();
        _handler = new ApproveRegistrationCommandHandler(_regRepo.Object, _regService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_RegistrationNotFound_ReturnsFailure()
    {
        _regRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.EventRegistration?)null);

        var result = await _handler.Handle(new ApproveRegistrationCommand { RegistrationId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Registration not found");
    }

    [Fact]
    public async Task Handle_PendingRegistration_Approved()
    {
        var reg = EventDataFixture.CreatePendingRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new ApproveRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        reg.Status.Should().Be(EventRegistrationStatus.Approved);
        reg.ApprovalDate.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WaitlistedRegistration_WithCapacity_Approved()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        var reg = EventDataFixture.CreateWaitlistedRegistration();
        reg.Event = evt;
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);
        _regService.Setup(x => x.IsCapacityAvailableAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _handler.Handle(new ApproveRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        reg.Status.Should().Be(EventRegistrationStatus.Approved);
    }

    [Fact]
    public async Task Handle_WaitlistedRegistration_WithoutCapacity_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        var reg = EventDataFixture.CreateWaitlistedRegistration();
        reg.Event = evt;
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);
        _regService.Setup(x => x.IsCapacityAvailableAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new ApproveRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("maximum capacity");
    }

    [Fact]
    public async Task Handle_AlreadyApproved_ReturnsFailure()
    {
        var reg = EventDataFixture.CreateApprovedRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new ApproveRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only pending or waitlisted");
    }

    [Fact]
    public async Task Handle_CancelledRegistration_ReturnsFailure()
    {
        var reg = EventDataFixture.CreateCancelledRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new ApproveRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only pending or waitlisted");
    }
}
