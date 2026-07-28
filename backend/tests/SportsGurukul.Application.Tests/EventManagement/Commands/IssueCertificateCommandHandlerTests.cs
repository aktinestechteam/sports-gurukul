using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.IssueCertificate;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class IssueCertificateCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventCertificateService> _certService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<IssueCertificateCommandHandler>> _logger;
    private readonly IssueCertificateCommandHandler _handler;

    public IssueCertificateCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _certService = EventMockFactory.CreateCertificateService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<IssueCertificateCommandHandler>();
        _handler = new IssueCertificateCommandHandler(_eventRepo.Object, _certService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new IssueCertificateCommand { EventId = Guid.NewGuid(), ParticipantId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_ParticipantNotFound_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new IssueCertificateCommand { EventId = evt.Id, ParticipantId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Participant not found");
    }

    [Fact]
    public async Task Handle_ValidIssue_CreatesCertificate()
    {
        var participantId = Guid.NewGuid();
        var evt = EventDataFixture.CreateCompletedEvent();
        evt.Participants = new List<Domain.Entities.EventParticipant>
        {
            EventDataFixture.CreateParticipant(participantId, evt.Id)
        };
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _certService.Setup(x => x.GenerateCertificateNumberAsync(It.IsAny<CancellationToken>())).ReturnsAsync("CERT-001");

        var result = await _handler.Handle(new IssueCertificateCommand { EventId = evt.Id, ParticipantId = participantId, CertificateType = "Merit" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CertificateType.Should().Be("Merit");
        result.Value.CertificateNumber.Should().Be("CERT-001");
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
