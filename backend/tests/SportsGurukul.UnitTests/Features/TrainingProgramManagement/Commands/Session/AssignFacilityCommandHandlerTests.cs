using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.AssignFacility;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Session;

public class AssignFacilityCommandHandlerTests
{
    private readonly Mock<ILogger<AssignFacilityCommandHandler>> _loggerMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<IFacilityRepository> _facilityRepositoryMock;
    private readonly AssignFacilityCommandHandler _handler;

    public AssignFacilityCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<AssignFacilityCommandHandler>>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _facilityRepositoryMock = new Mock<IFacilityRepository>();

        _handler = new AssignFacilityCommandHandler(
            _loggerMock.Object,
            _sessionRepositoryMock.Object,
            _facilityRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidFacility()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var facility = TestHelpers.CreateTestFacility();
        var command = new AssignFacilityCommand(session.Id, facility.Id);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _facilityRepositoryMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        _sessionRepositoryMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession>());

        var updatedSession = TestHelpers.CreateTestSession(
            id: session.Id, batchId: session.BatchId, coachId: session.CoachId);
        updatedSession.FacilityId = facility.Id;

        _sessionRepositoryMock.SetupSequence(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session)
            .ReturnsAsync(updatedSession);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NullFacility()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        session.FacilityId = Guid.NewGuid();
        var command = new AssignFacilityCommand(session.Id, null);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var updatedSession = TestHelpers.CreateTestSession(
            id: session.Id, batchId: session.BatchId, coachId: session.CoachId);
        updatedSession.FacilityId = null;

        _sessionRepositoryMock.SetupSequence(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session)
            .ReturnsAsync(updatedSession);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _facilityRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _sessionRepositoryMock.Verify(r => r.GetByFacilityIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionNotFound()
    {
        var facilityId = Guid.NewGuid();
        var command = new AssignFacilityCommand(Guid.NewGuid(), facilityId);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingSession?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _facilityRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_FacilityNotFound()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var facilityId = Guid.NewGuid();
        var command = new AssignFacilityCommand(session.Id, facilityId);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _facilityRepositoryMock.Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facility?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_FacilityDoubleBooked()
    {
        var session = TestHelpers.CreateTestSession(status: SessionStatus.Scheduled);
        var facility = TestHelpers.CreateTestFacility();
        var command = new AssignFacilityCommand(session.Id, facility.Id);

        var existingSession = TestHelpers.CreateTestSession();
        existingSession.Id = Guid.NewGuid();
        existingSession.FacilityId = facility.Id;
        existingSession.SessionDate = session.SessionDate;
        existingSession.StartTime = session.StartTime;
        existingSession.EndTime = session.EndTime;

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        _facilityRepositoryMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        _sessionRepositoryMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession> { existingSession });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already booked");
        _sessionRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingSession>()), Times.Never);
    }
}
