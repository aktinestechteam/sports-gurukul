using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.UpdateAttendance;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Attendance;

public class UpdateAttendanceCommandHandlerTests
{
    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<ILogger<UpdateAttendanceCommandHandler>> _loggerMock;
    private readonly UpdateAttendanceCommandHandler _handler;

    public UpdateAttendanceCommandHandlerTests()
    {
        _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _loggerMock = new Mock<ILogger<UpdateAttendanceCommandHandler>>();

        _handler = new UpdateAttendanceCommandHandler(
            _attendanceRepositoryMock.Object,
            _sessionRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static UpdateAttendanceCommand CreateValidCommand(
        Guid? attendanceId = null,
        AttendanceStatus status = AttendanceStatus.Present,
        string? remarks = "Updated remark") => new()
    {
        AttendanceId = attendanceId ?? Guid.NewGuid(),
        Status = status,
        Remarks = remarks
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidUpdate()
    {
        var attendanceId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(id: attendanceId, sessionId: sessionId, status: AttendanceStatus.Absent);
        var command = CreateValidCommand(attendanceId, AttendanceStatus.Present, "Updated");

        _attendanceRepositoryMock.Setup(r => r.GetByIdAsync(attendanceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);
        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestSession(id: sessionId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(attendanceId);
        _attendanceRepositoryMock.Verify(r => r.Update(attendance), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AttendanceNotFound()
    {
        var command = CreateValidCommand();

        _attendanceRepositoryMock.Setup(r => r.GetByIdAsync(command.AttendanceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Attendance?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Attendance record not found");
        _attendanceRepositoryMock.Verify(r => r.Update(It.IsAny<Domain.Entities.Attendance>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateStatus_When_ValidCommand()
    {
        var attendanceId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var attendance = TestHelpers.CreateTestAttendance(id: attendanceId, sessionId: sessionId, status: AttendanceStatus.Absent);
        var command = CreateValidCommand(attendanceId, AttendanceStatus.Late, "Arrived late");

        _attendanceRepositoryMock.Setup(r => r.GetByIdAsync(attendanceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendance);
        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestSession(id: sessionId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AttendanceStatus.Should().Be("Late");
        result.Value.Remarks.Should().Be("Arrived late");
    }
}
