using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckOutAthlete;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Attendance;

using AttendanceEntity = SportsGurukul.Domain.Entities.Attendance;

public class CheckOutAthleteCommandHandlerTests
{
    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<ILogger<CheckOutAthleteCommandHandler>> _loggerMock;
    private readonly CheckOutAthleteCommandHandler _handler;

    public CheckOutAthleteCommandHandlerTests()
    {
        _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _loggerMock = new Mock<ILogger<CheckOutAthleteCommandHandler>>();

        _handler = new CheckOutAthleteCommandHandler(
            _attendanceRepositoryMock.Object,
            _sessionRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static CheckOutAthleteCommand CreateValidCommand(Guid? sessionId = null, Guid? athleteId = null) => new()
    {
        SessionId = sessionId ?? Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid()
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCheckOut()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(sessionId: sessionId, athleteId: athleteId, status: AttendanceStatus.Present);
        attendance.CheckInTime = DateTime.UtcNow.AddMinutes(-60);
        attendance.CheckOutTime = null;
        var session = TestHelpers.CreateTestSession(id: sessionId);
        var command = CreateValidCommand(sessionId, athleteId);

        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);
        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CheckOutTime.Should().NotBeNull();
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
        result.Error.Should().Be("No attendance record found for this session");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NotCheckedIn()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(sessionId: sessionId, athleteId: athleteId);
        attendance.CheckInTime = null;
        attendance.CheckOutTime = null;
        var command = CreateValidCommand(sessionId, athleteId);

        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete has not checked in yet");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AlreadyCheckedOut()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(sessionId: sessionId, athleteId: athleteId, status: AttendanceStatus.Present);
        attendance.CheckInTime = DateTime.UtcNow.AddMinutes(-120);
        attendance.CheckOutTime = DateTime.UtcNow.AddMinutes(-60);
        var command = CreateValidCommand(sessionId, athleteId);

        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete has already checked out from this session");
    }
}
