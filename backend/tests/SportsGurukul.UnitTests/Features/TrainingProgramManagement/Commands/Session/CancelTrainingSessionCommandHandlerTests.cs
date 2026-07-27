using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CancelTrainingSession;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Session;

public class CancelTrainingSessionCommandHandlerTests
{
    private readonly Mock<ILogger<CancelTrainingSessionCommandHandler>> _loggerMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly CancelTrainingSessionCommandHandler _handler;

    public CancelTrainingSessionCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CancelTrainingSessionCommandHandler>>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();

        _handler = new CancelTrainingSessionCommandHandler(
            _loggerMock.Object,
            _sessionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_SessionIsScheduled()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var command = new CancelTrainingSessionCommand(session.Id);

        var cancelledSession = TestHelpers.CreateTestSession(
            id: session.Id, batchId: session.BatchId, coachId: session.CoachId, status: SessionStatus.Cancelled);

        _sessionRepositoryMock.SetupSequence(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session)
            .ReturnsAsync(cancelledSession);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(SessionStatus.Cancelled.ToString());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionNotFound()
    {
        var command = new CancelTrainingSessionCommand(Guid.NewGuid());

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
        var command = new CancelTrainingSessionCommand(session.Id);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Scheduled");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }
}
