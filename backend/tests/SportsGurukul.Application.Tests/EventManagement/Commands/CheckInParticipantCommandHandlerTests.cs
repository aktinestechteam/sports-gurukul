using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class CheckInParticipantCommandHandlerTests
{
    private readonly Mock<IEventAttendanceRepository> _attendanceRepo;
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventAttendanceService> _attendanceService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<CheckInParticipantCommandHandler>> _logger;
    private readonly CheckInParticipantCommandHandler _handler;

    public CheckInParticipantCommandHandlerTests()
    {
        _attendanceRepo = EventMockFactory.CreateAttendanceRepository();
        _eventRepo = EventMockFactory.CreateEventRepository();
        _attendanceService = EventMockFactory.CreateAttendanceService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<CheckInParticipantCommandHandler>();
        _handler = new CheckInParticipantCommandHandler(_attendanceRepo.Object, _eventRepo.Object, _attendanceService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new CheckInParticipantCommand { EventId = Guid.NewGuid(), ParticipantId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_ParticipantNotFound_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new CheckInParticipantCommand { EventId = evt.Id, ParticipantId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Participant not found");
    }

    [Fact]
    public async Task Handle_ParticipantNotEligible_ReturnsFailure()
    {
        var participantId = Guid.NewGuid();
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.Participants = new List<Domain.Entities.EventParticipant>
        {
            EventDataFixture.CreateParticipant(participantId, evt.Id, EventAttendanceStatus.Absent)
        };
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _attendanceService.Setup(x => x.CanCheckInAsync(It.IsAny<Domain.Entities.EventParticipant>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new CheckInParticipantCommand { EventId = evt.Id, ParticipantId = participantId }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not eligible");
    }

    [Fact]
    public async Task Handle_NewCheckIn_CreatesAttendance()
    {
        var participantId = Guid.NewGuid();
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.Participants = new List<Domain.Entities.EventParticipant>
        {
            EventDataFixture.CreateParticipant(participantId, evt.Id, EventAttendanceStatus.Registered)
        };
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _attendanceService.Setup(x => x.CanCheckInAsync(It.IsAny<Domain.Entities.EventParticipant>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _handler.Handle(new CheckInParticipantCommand { EventId = evt.Id, ParticipantId = participantId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(EventAttendanceStatus.CheckedIn.ToString());
        _attendanceRepo.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.EventAttendance>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CheckInWithSession_CreatesSessionAttendance()
    {
        var participantId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.Participants = new List<Domain.Entities.EventParticipant>
        {
            EventDataFixture.CreateParticipant(participantId, evt.Id, EventAttendanceStatus.Registered)
        };
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _attendanceService.Setup(x => x.CanCheckInAsync(It.IsAny<Domain.Entities.EventParticipant>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _attendanceRepo.Setup(x => x.GetBySessionAndParticipantAsync(sessionId, participantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.EventAttendance?)null);

        var result = await _handler.Handle(new CheckInParticipantCommand { EventId = evt.Id, ParticipantId = participantId, SessionId = sessionId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _attendanceRepo.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.EventAttendance>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ParticipantAttendanceStatusUpdated()
    {
        var participantId = Guid.NewGuid();
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        var participant = EventDataFixture.CreateParticipant(participantId, evt.Id, EventAttendanceStatus.Registered);
        evt.Participants = new List<Domain.Entities.EventParticipant> { participant };
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _attendanceService.Setup(x => x.CanCheckInAsync(participant, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await _handler.Handle(new CheckInParticipantCommand { EventId = evt.Id, ParticipantId = participantId }, CancellationToken.None);

        participant.AttendanceStatus.Should().Be(EventAttendanceStatus.CheckedIn);
    }
}
