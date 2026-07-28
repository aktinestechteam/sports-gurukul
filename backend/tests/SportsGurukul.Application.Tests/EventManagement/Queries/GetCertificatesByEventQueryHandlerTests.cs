using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.GetCertificatesByEvent;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class GetCertificatesByEventQueryHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<ILogger<GetCertificatesByEventQueryHandler>> _logger;
    private readonly GetCertificatesByEventQueryHandler _handler;

    public GetCertificatesByEventQueryHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _logger = EventMockFactory.CreateLogger<GetCertificatesByEventQueryHandler>();
        _handler = new GetCertificatesByEventQueryHandler(_eventRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new GetCertificatesByEventQuery { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_EventWithCerts_ReturnsCerts()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        var participant = EventDataFixture.CreateParticipant();
        evt.Participants = new List<Domain.Entities.EventParticipant> { participant };
        evt.Certificates = new List<Domain.Entities.EventCertificate>
        {
            EventDataFixture.CreateCertificate(eventId: evt.Id, participantId: participant.Id)
        };
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new GetCertificatesByEventQuery { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].EventName.Should().Be(evt.EventName);
    }
}
