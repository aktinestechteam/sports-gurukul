using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.GenerateCertificates;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class GenerateCertificatesCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventCertificateService> _certService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<GenerateCertificatesCommandHandler>> _logger;
    private readonly GenerateCertificatesCommandHandler _handler;

    public GenerateCertificatesCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _certService = EventMockFactory.CreateCertificateService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<GenerateCertificatesCommandHandler>();
        _handler = new GenerateCertificatesCommandHandler(_eventRepo.Object, _certService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new GenerateCertificatesCommand { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_EventNotCompleted_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new GenerateCertificatesCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("can only be generated for completed events");
    }

    [Fact]
    public async Task Handle_NoEligibleParticipants_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _certService.Setup(x => x.GetEligibleParticipantsAsync(evt.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventParticipant>());

        var result = await _handler.Handle(new GenerateCertificatesCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("No eligible participants");
    }

    [Fact]
    public async Task Handle_ValidGeneration_GeneratesCerts()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        var participant = EventDataFixture.CreateParticipant();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _certService.Setup(x => x.GetEligibleParticipantsAsync(evt.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventParticipant> { participant });
        _certService.Setup(x => x.GenerateCertificateNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("CERT-001");

        var result = await _handler.Handle(new GenerateCertificatesCommand { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
