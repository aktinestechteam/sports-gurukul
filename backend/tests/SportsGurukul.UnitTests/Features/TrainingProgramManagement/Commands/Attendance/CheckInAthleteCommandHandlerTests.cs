using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckInAthlete;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Attendance;

using AttendanceEntity = SportsGurukul.Domain.Entities.Attendance;

public class CheckInAthleteCommandHandlerTests
{
    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<ILogger<CheckInAthleteCommandHandler>> _loggerMock;
    private readonly CheckInAthleteCommandHandler _handler;

    public CheckInAthleteCommandHandlerTests()
    {
        _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _loggerMock = new Mock<ILogger<CheckInAthleteCommandHandler>>();

        _handler = new CheckInAthleteCommandHandler(
            _attendanceRepositoryMock.Object,
            _sessionRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static CheckInAthleteCommand CreateValidCommand(Guid? sessionId = null, Guid? athleteId = null) => new()
    {
        SessionId = sessionId ?? Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid()
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCheckIn()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(sessionId: sessionId, athleteId: athleteId, status: AttendanceStatus.Absent);
        attendance.CheckInTime = null;
        var session = TestHelpers.CreateTestSession(id: sessionId);
        session.SessionDate = DateTime.UtcNow.AddDays(1);
        var command = CreateValidCommand(sessionId, athleteId);

        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);
        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CheckInTime.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoAttendanceRecord()
    {
        var command = CreateValidCommand();

        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(command.SessionId, command.AthleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SportsGurukul.Domain.Entities.Attendance?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No attendance record found. Athlete must be marked for attendance first");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AlreadyCheckedIn()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(sessionId: sessionId, athleteId: athleteId);
        attendance.CheckInTime = DateTime.UtcNow.AddMinutes(-30);
        var command = CreateValidCommand(sessionId, athleteId);

        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete has already checked in for this session");
    }

    [Fact]
    public async Task Handle_Should_SetPresentStatus_When_OnTime()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(sessionId: sessionId, athleteId: athleteId, status: AttendanceStatus.Absent);
        attendance.CheckInTime = null;
        var session = TestHelpers.CreateTestSession(id: sessionId);
        session.SessionDate = DateTime.UtcNow.AddDays(1);
        var command = CreateValidCommand(sessionId, athleteId);

        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);
        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AttendanceStatus.Should().Be("Present");
    }

    [Fact]
    public async Task Handle_Should_SetLateStatus_When_Late()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(sessionId: sessionId, athleteId: athleteId, status: AttendanceStatus.Absent);
        attendance.CheckInTime = null;
        var session = TestHelpers.CreateTestSession(id: sessionId);
        session.SessionDate = DateTime.UtcNow.AddDays(-1);
        var command = CreateValidCommand(sessionId, athleteId);

        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);
        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AttendanceStatus.Should().Be("Late");
    }
}
