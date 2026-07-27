using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.RescheduleTrainingSession;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Session;

public class RescheduleTrainingSessionCommandHandlerTests
{
    private readonly Mock<ILogger<RescheduleTrainingSessionCommandHandler>> _loggerMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly RescheduleTrainingSessionCommandHandler _handler;

    public RescheduleTrainingSessionCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<RescheduleTrainingSessionCommandHandler>>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();

        _handler = new RescheduleTrainingSessionCommandHandler(
            _loggerMock.Object,
            _sessionRepositoryMock.Object);
    }

    private static RescheduleTrainingSessionCommand CreateValidCommand(
        Guid? sessionId = null,
        DateTime? sessionDate = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null) => new(
        Id: sessionId ?? Guid.NewGuid(),
        SessionDate: sessionDate ?? DateTime.UtcNow.AddDays(10),
        StartTime: startTime ?? new TimeSpan(10, 0, 0),
        EndTime: endTime ?? new TimeSpan(12, 0, 0));

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidReschedule()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var command = CreateValidCommand(sessionId: session.Id);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _sessionRepositoryMock.Setup(r => r.GetByCoachIdAsync(session.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession>());

        var rescheduledSession = TestHelpers.CreateTestSession(
            id: session.Id, batchId: session.BatchId, coachId: session.CoachId, status: SessionStatus.Scheduled);
        rescheduledSession.SessionDate = command.SessionDate;
        rescheduledSession.StartTime = command.StartTime;
        rescheduledSession.EndTime = command.EndTime;

        _sessionRepositoryMock.SetupSequence(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session)
            .ReturnsAsync(rescheduledSession);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionNotFound()
    {
        var command = CreateValidCommand();

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingSession?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionIsNotScheduled()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.InProgress);
        var command = CreateValidCommand(sessionId: session.Id);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Scheduled");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_StartTimeAfterEndTime()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var command = CreateValidCommand(
            sessionId: session.Id,
            startTime: new TimeSpan(14, 0, 0),
            endTime: new TimeSpan(9, 0, 0));

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Start time must be before end time");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CoachOverlap()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var command = CreateValidCommand(
            sessionId: session.Id,
            sessionDate: session.SessionDate,
            startTime: new TimeSpan(10, 0, 0),
            endTime: new TimeSpan(12, 0, 0));

        var overlappingSession = TestHelpers.CreateTestSession();
        overlappingSession.Id = Guid.NewGuid();
        overlappingSession.CoachId = session.CoachId;
        overlappingSession.SessionDate = session.SessionDate;
        overlappingSession.StartTime = new TimeSpan(9, 0, 0);
        overlappingSession.EndTime = new TimeSpan(11, 0, 0);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _sessionRepositoryMock.Setup(r => r.GetByCoachIdAsync(session.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession> { overlappingSession });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("overlapping");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_FacilityOverlap()
    {
        var facilityId = Guid.NewGuid();
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        session.FacilityId = facilityId;

        var command = CreateValidCommand(
            sessionId: session.Id,
            sessionDate: session.SessionDate,
            startTime: new TimeSpan(10, 0, 0),
            endTime: new TimeSpan(12, 0, 0));

        var overlappingSession = TestHelpers.CreateTestSession();
        overlappingSession.Id = Guid.NewGuid();
        overlappingSession.FacilityId = facilityId;
        overlappingSession.SessionDate = session.SessionDate;
        overlappingSession.StartTime = new TimeSpan(9, 0, 0);
        overlappingSession.EndTime = new TimeSpan(11, 0, 0);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _sessionRepositoryMock.Setup(r => r.GetByCoachIdAsync(session.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession>());

        _sessionRepositoryMock.Setup(r => r.GetByFacilityIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession> { overlappingSession });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already booked");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }
}
