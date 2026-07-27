using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CompleteTrainingSession;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Session;

public class CompleteTrainingSessionCommandHandlerTests
{
    private readonly Mock<ILogger<CompleteTrainingSessionCommandHandler>> _loggerMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly CompleteTrainingSessionCommandHandler _handler;

    public CompleteTrainingSessionCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CompleteTrainingSessionCommandHandler>>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();

        _handler = new CompleteTrainingSessionCommandHandler(
            _loggerMock.Object,
            _sessionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_SessionIsScheduled()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var command = new CompleteTrainingSessionCommand(session.Id);

        var completedSession = TestHelpers.CreateTestSession(
            id: session.Id, batchId: session.BatchId, coachId: session.CoachId, status: SessionStatus.Completed);

        _sessionRepositoryMock.SetupSequence(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session)
            .ReturnsAsync(completedSession);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(SessionStatus.Completed.ToString());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_SessionIsInProgress()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.InProgress);
        var command = new CompleteTrainingSessionCommand(session.Id);

        var completedSession = TestHelpers.CreateTestSession(
            id: session.Id, batchId: session.BatchId, coachId: session.CoachId, status: SessionStatus.Completed);

        _sessionRepositoryMock.SetupSequence(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session)
            .ReturnsAsync(completedSession);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(SessionStatus.Completed.ToString());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionNotFound()
    {
        var command = new CompleteTrainingSessionCommand(Guid.NewGuid());

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingSession?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionIsAlreadyCompleted()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Completed);
        var command = new CompleteTrainingSessionCommand(session.Id);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Scheduled or InProgress");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionIsCancelled()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Cancelled);
        var command = new CompleteTrainingSessionCommand(session.Id);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Scheduled or InProgress");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }
}
