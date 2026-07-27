using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.MarkAttendance;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Attendance;

using AttendanceEntity = SportsGurukul.Domain.Entities.Attendance;

public class MarkAttendanceCommandHandlerTests
{
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ILogger<MarkAttendanceCommandHandler>> _loggerMock;
    private readonly MarkAttendanceCommandHandler _handler;

    public MarkAttendanceCommandHandlerTests()
    {
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _loggerMock = new Mock<ILogger<MarkAttendanceCommandHandler>>();

        _handler = new MarkAttendanceCommandHandler(
            _sessionRepositoryMock.Object,
            _attendanceRepositoryMock.Object,
            _batchRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static MarkAttendanceCommand CreateValidCommand(
        Guid? sessionId = null,
        Guid? athleteId = null,
        AttendanceStatus status = AttendanceStatus.Present) => new()
    {
        SessionId = sessionId ?? Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid(),
        Status = status,
        Remarks = "Good performance"
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidAttendance()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        var enrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var command = CreateValidCommand(sessionId, athleteId);

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AttendanceEntity?)null);
        _attendanceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AttendanceEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AttendanceEntity a, CancellationToken _) => a);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.SessionId.Should().Be(sessionId);
        result.Value.AthleteId.Should().Be(athleteId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionNotFound()
    {
        var command = CreateValidCommand();

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(command.SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingSession?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Session not found");
        _batchRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchNotFound()
    {
        var sessionId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var command = CreateValidCommand(sessionId);

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Associated batch not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AthleteNotEnrolled()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment>();
        var command = CreateValidCommand(sessionId, athleteId);

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete is not enrolled in the associated batch");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AlreadyMarked()
    {
        var sessionId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        var enrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var existingAttendance = TestHelpers.CreateTestAttendance(sessionId: sessionId, athleteId: athleteId);
        var command = CreateValidCommand(sessionId, athleteId);

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _attendanceRepositoryMock.Setup(r => r.GetBySessionAndAthleteAsync(sessionId, athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAttendance);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Attendance already marked for this session");
        _attendanceRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AttendanceEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
