using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Services;

public class EventCertificateServiceTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventAttendanceRepository> _attendanceRepo;
    private readonly Mock<ILogger<EventCertificateService>> _logger;
    private readonly EventCertificateService _service;

    public EventCertificateServiceTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _attendanceRepo = EventMockFactory.CreateAttendanceRepository();
        _logger = EventMockFactory.CreateLogger<EventCertificateService>();
        _service = new EventCertificateService(_eventRepo.Object, _attendanceRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task GenerateCertificateNumberAsync_ReturnsFormattedNumber()
    {
        var result = await _service.GenerateCertificateNumberAsync();
        result.Should().StartWith("CERT-");
    }

    [Fact]
    public async Task IsEligibleForCertificateAsync_EventNotCompleted_ReturnsFalse()
    {
        var participant = EventDataFixture.CreateParticipant();
        participant.Event = EventDataFixture.CreateDraftEvent();

        var result = await _service.IsEligibleForCertificateAsync(participant);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsEligibleForCertificateAsync_NoAttendances_ReturnsFalse()
    {
        var participant = EventDataFixture.CreateParticipant();
        participant.Event = EventDataFixture.CreateCompletedEvent();
        _attendanceRepo.Setup(x => x.GetByParticipantIdAsync(participant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventAttendance>());

        var result = await _service.IsEligibleForCertificateAsync(participant);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsEligibleForCertificateAsync_HighAttendanceRate_ReturnsTrue()
    {
        var participant = EventDataFixture.CreateParticipant();
        participant.Event = EventDataFixture.CreateCompletedEvent();
        var attendances = new List<Domain.Entities.EventAttendance>
        {
            EventDataFixture.CreateAttendance(participantId: participant.Id, status: EventAttendanceStatus.Present),
            EventDataFixture.CreateAttendance(participantId: participant.Id, status: EventAttendanceStatus.Present),
            EventDataFixture.CreateAttendance(participantId: participant.Id, status: EventAttendanceStatus.Present),
            EventDataFixture.CreateAttendance(participantId: participant.Id, status: EventAttendanceStatus.Absent)
        };
        _attendanceRepo.Setup(x => x.GetByParticipantIdAsync(participant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendances);

        var result = await _service.IsEligibleForCertificateAsync(participant);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsEligibleForCertificateAsync_LowAttendanceRate_ReturnsFalse()
    {
        var participant = EventDataFixture.CreateParticipant();
        participant.Event = EventDataFixture.CreateCompletedEvent();
        var attendances = new List<Domain.Entities.EventAttendance>
        {
            EventDataFixture.CreateAttendance(participantId: participant.Id, status: EventAttendanceStatus.Present),
            EventDataFixture.CreateAttendance(participantId: participant.Id, status: EventAttendanceStatus.Absent),
            EventDataFixture.CreateAttendance(participantId: participant.Id, status: EventAttendanceStatus.Absent),
            EventDataFixture.CreateAttendance(participantId: participant.Id, status: EventAttendanceStatus.Absent)
        };
        _attendanceRepo.Setup(x => x.GetByParticipantIdAsync(participant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendances);

        var result = await _service.IsEligibleForCertificateAsync(participant);
        result.Should().BeFalse();
    }
}
