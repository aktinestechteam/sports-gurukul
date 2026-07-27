using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.UpdateTrainingSession;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Session;

public class UpdateTrainingSessionCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateTrainingSessionCommandHandler>> _loggerMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly UpdateTrainingSessionCommandHandler _handler;

    public UpdateTrainingSessionCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateTrainingSessionCommandHandler>>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();

        _handler = new UpdateTrainingSessionCommandHandler(
            _loggerMock.Object,
            _sessionRepositoryMock.Object);
    }

    private static UpdateTrainingSessionCommand CreateValidCommand(
        Guid? sessionId = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null) => new(
        Id: sessionId ?? Guid.NewGuid(),
        SessionTitle: "Updated Session",
        SessionType: SessionType.Theory,
        SessionDate: DateTime.UtcNow.AddDays(5),
        StartTime: startTime ?? new TimeSpan(10, 0, 0),
        EndTime: endTime ?? new TimeSpan(12, 0, 0));

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_SessionIsScheduled()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var command = CreateValidCommand(sessionId: session.Id);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var updatedSession = TestHelpers.CreateTestSession(id: session.Id, batchId: session.BatchId, coachId: session.CoachId);
        updatedSession.SessionTitle = command.SessionTitle;
        updatedSession.SessionType = command.SessionType;
        updatedSession.SessionDate = command.SessionDate;
        updatedSession.StartTime = command.StartTime;
        updatedSession.EndTime = command.EndTime;

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionNotFound()
    {
        var command = CreateValidCommand();

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.TrainingSession?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<Domain.Entities.TrainingSession>()), Times.Never);
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
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<Domain.Entities.TrainingSession>()), Times.Never);
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
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<Domain.Entities.TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateFields_When_ValidCommand()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var command = CreateValidCommand(sessionId: session.Id);

        var updatedSession = TestHelpers.CreateTestSession(id: session.Id, batchId: session.BatchId, coachId: session.CoachId);
        updatedSession.SessionTitle = command.SessionTitle;
        updatedSession.SessionType = command.SessionType;
        updatedSession.SessionDate = command.SessionDate;
        updatedSession.StartTime = command.StartTime;
        updatedSession.EndTime = command.EndTime;

        _sessionRepositoryMock.SetupSequence(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session)
            .ReturnsAsync(updatedSession);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<Domain.Entities.TrainingSession>()), Times.Once);
        session.SessionTitle.Should().Be(command.SessionTitle);
        session.SessionType.Should().Be(command.SessionType);
        session.SessionDate.Should().Be(command.SessionDate);
        session.StartTime.Should().Be(command.StartTime);
        session.EndTime.Should().Be(command.EndTime);
        session.UpdatedAt.Should().NotBeNull();
    }
}
