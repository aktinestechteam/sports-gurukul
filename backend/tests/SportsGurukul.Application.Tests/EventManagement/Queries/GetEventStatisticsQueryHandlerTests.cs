using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.GetEventStatistics;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class GetEventStatisticsQueryHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<ILogger<GetEventStatisticsQueryHandler>> _logger;
    private readonly GetEventStatisticsQueryHandler _handler;

    public GetEventStatisticsQueryHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _logger = EventMockFactory.CreateLogger<GetEventStatisticsQueryHandler>();
        _handler = new GetEventStatisticsQueryHandler(_eventRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new GetEventStatisticsQuery { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_EventExists_ReturnsStatistics()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        evt.Registrations = new List<Domain.Entities.EventRegistration>
        {
            EventDataFixture.CreateApprovedRegistration(),
            EventDataFixture.CreatePendingRegistration(),
            EventDataFixture.CreateCancelledRegistration()
        };
        evt.Participants = new List<Domain.Entities.EventParticipant> { EventDataFixture.CreateParticipant() };
        evt.Sessions = new List<Domain.Entities.EventSession> { EventDataFixture.CreateSession() };
        evt.Certificates = new List<Domain.Entities.EventCertificate> { EventDataFixture.CreateCertificate() };
        evt.Feedbacks = new List<Domain.Entities.EventFeedback> { EventDataFixture.CreateFeedback() };
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new GetEventStatisticsQuery { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalRegistrations.Should().Be(3);
        result.Value.ApprovedRegistrations.Should().Be(1);
        result.Value.PendingRegistrations.Should().Be(1);
        result.Value.CancelledRegistrations.Should().Be(1);
        result.Value.TotalParticipants.Should().Be(1);
        result.Value.TotalSessions.Should().Be(1);
        result.Value.CertificatesIssued.Should().Be(1);
        result.Value.FeedbackCount.Should().Be(1);
    }
}
