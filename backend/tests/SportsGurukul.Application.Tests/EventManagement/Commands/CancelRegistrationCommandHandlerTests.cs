using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.CancelRegistration;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class CancelRegistrationCommandHandlerTests
{
    private readonly Mock<IEventRegistrationRepository> _regRepo;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<CancelRegistrationCommandHandler>> _logger;
    private readonly CancelRegistrationCommandHandler _handler;

    public CancelRegistrationCommandHandlerTests()
    {
        _regRepo = EventMockFactory.CreateRegistrationRepository();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<CancelRegistrationCommandHandler>();
        _handler = new CancelRegistrationCommandHandler(_regRepo.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_RegistrationNotFound_ReturnsFailure()
    {
        _regRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.EventRegistration?)null);

        var result = await _handler.Handle(new CancelRegistrationCommand { RegistrationId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Registration not found");
    }

    [Fact]
    public async Task Handle_AlreadyCancelled_ReturnsFailure()
    {
        var reg = EventDataFixture.CreateCancelledRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new CancelRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already cancelled");
    }

    [Fact]
    public async Task Handle_PendingRegistration_Cancelled()
    {
        var reg = EventDataFixture.CreatePendingRegistration();
        reg.Event = EventDataFixture.CreateRegistrationOpenEvent();
        _regRepo.Setup(x => x.GetWithDetailsAsync(reg.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reg);

        var result = await _handler.Handle(new CancelRegistrationCommand { RegistrationId = reg.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        reg.Status.Should().Be(EventRegistrationStatus.Cancelled);
    }
}
