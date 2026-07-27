using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CreateTrainingSession;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Session;

public class CreateTrainingSessionCommandHandlerTests
{
    private readonly Mock<ILogger<CreateTrainingSessionCommandHandler>> _loggerMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<ICoachRepository> _coachRepositoryMock;
    private readonly Mock<IFacilityRepository> _facilityRepositoryMock;
    private readonly CreateTrainingSessionCommandHandler _handler;

    public CreateTrainingSessionCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateTrainingSessionCommandHandler>>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _coachRepositoryMock = new Mock<ICoachRepository>();
        _facilityRepositoryMock = new Mock<IFacilityRepository>();

        _handler = new CreateTrainingSessionCommandHandler(
            _loggerMock.Object,
            _batchRepositoryMock.Object,
            _sessionRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _facilityRepositoryMock.Object);
    }

    private static CreateTrainingSessionCommand CreateValidCommand(
        Guid? batchId = null,
        Guid? coachId = null,
        Guid? facilityId = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null) => new(
        BatchId: batchId ?? Guid.NewGuid(),
        SessionTitle: "Test Session",
        SessionType: SessionType.Practice,
        SessionDate: DateTime.UtcNow.AddDays(1),
        StartTime: startTime ?? new TimeSpan(9, 0, 0),
        EndTime: endTime ?? new TimeSpan(11, 0, 0),
        FacilityId: facilityId,
        CoachId: coachId ?? Guid.NewGuid());

    private void SetupValidMocks(
        TrainingBatch batch,
        Coach coach,
        CreateTrainingSessionCommand command,
        TrainingSession? returnedSession = null,
        Facility? facility = null)
    {
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        _sessionRepositoryMock.Setup(r => r.GetByCoachIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession>());

        _sessionRepositoryMock.Setup(r => r.IsSessionCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        if (command.FacilityId.HasValue && facility != null)
        {
            _facilityRepositoryMock.Setup(r => r.GetByIdAsync(command.FacilityId.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(facility);

            _sessionRepositoryMock.Setup(r => r.GetByFacilityIdAsync(command.FacilityId.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingSession>());
        }

        var session = returnedSession ?? TestHelpers.CreateTestSession(
            batchId: batch.Id,
            coachId: coach.Id);

        _sessionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingSession s, CancellationToken _) => s);

        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCommand()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Active);
        var coach = TestHelpers.CreateTestCoach(batch.CoachId);
        var facility = TestHelpers.CreateTestFacility();
        var command = CreateValidCommand(batchId: batch.Id, coachId: coach.Id, facilityId: facility.Id);

        SetupValidMocks(batch, coach, command, facility: facility);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchNotFound()
    {
        var command = CreateValidCommand();

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _coachRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchIsNotActive()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Waitlisted);
        var coach = TestHelpers.CreateTestCoach();
        var command = CreateValidCommand(batchId: batch.Id, coachId: coach.Id);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Active batches");
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CoachNotFound()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Active);
        var command = CreateValidCommand(batchId: batch.Id);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_FacilityNotFound()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Active);
        var coach = TestHelpers.CreateTestCoach(batch.CoachId);
        var facilityId = Guid.NewGuid();
        var command = CreateValidCommand(batchId: batch.Id, coachId: coach.Id, facilityId: facilityId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        _facilityRepositoryMock.Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facility?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_FacilityOverlap()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Active);
        var coach = TestHelpers.CreateTestCoach(batch.CoachId);
        var facility = TestHelpers.CreateTestFacility();
        var sessionDate = DateTime.UtcNow.AddDays(1);
        var command = CreateValidCommand(
            batchId: batch.Id, coachId: coach.Id, facilityId: facility.Id,
            startTime: new TimeSpan(10, 0, 0), endTime: new TimeSpan(12, 0, 0));

        var overlappingSession = TestHelpers.CreateTestSession();
        overlappingSession.SessionDate = sessionDate;
        overlappingSession.StartTime = new TimeSpan(9, 0, 0);
        overlappingSession.EndTime = new TimeSpan(11, 0, 0);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        _facilityRepositoryMock.Setup(r => r.GetByIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        _sessionRepositoryMock.Setup(r => r.GetByFacilityIdAsync(facility.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession> { overlappingSession });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already booked");
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CoachOverlap()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Active);
        var coach = TestHelpers.CreateTestCoach(batch.CoachId);
        var sessionDate = DateTime.UtcNow.AddDays(1);
        var command = CreateValidCommand(
            batchId: batch.Id, coachId: coach.Id,
            startTime: new TimeSpan(10, 0, 0), endTime: new TimeSpan(12, 0, 0));

        var overlappingSession = TestHelpers.CreateTestSession(coachId: coach.Id);
        overlappingSession.SessionDate = sessionDate;
        overlappingSession.StartTime = new TimeSpan(9, 0, 0);
        overlappingSession.EndTime = new TimeSpan(11, 0, 0);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        _sessionRepositoryMock.Setup(r => r.GetByCoachIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession> { overlappingSession });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("overlapping");
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_StartTimeAfterEndTime()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Active);
        var coach = TestHelpers.CreateTestCoach(batch.CoachId);
        var command = CreateValidCommand(
            batchId: batch.Id, coachId: coach.Id,
            startTime: new TimeSpan(14, 0, 0), endTime: new TimeSpan(9, 0, 0));

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        _sessionRepositoryMock.Setup(r => r.GetByCoachIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Start time must be before end time");
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_CreateSession_When_NoFacilityProvided()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Active);
        var coach = TestHelpers.CreateTestCoach(batch.CoachId);
        var command = CreateValidCommand(batchId: batch.Id, coachId: coach.Id, facilityId: null);

        SetupValidMocks(batch, coach, command);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _facilityRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _sessionRepositoryMock.Verify(r => r.GetByFacilityIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_GenerateSessionCode_When_Creating()
    {
        var batch = TestHelpers.CreateTestBatch(status: BatchStatus.Active);
        var coach = TestHelpers.CreateTestCoach(batch.CoachId);
        var command = CreateValidCommand(batchId: batch.Id, coachId: coach.Id);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        _sessionRepositoryMock.Setup(r => r.GetByCoachIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingSession>());

        _sessionRepositoryMock.Setup(r => r.IsSessionCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        TrainingSession? capturedSession = null;
        _sessionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingSession>(), It.IsAny<CancellationToken>()))
            .Callback<TrainingSession, CancellationToken>((s, _) => capturedSession = s)
            .ReturnsAsync((TrainingSession s, CancellationToken _) => s);

        var returnedSession = TestHelpers.CreateTestSession(batchId: batch.Id, coachId: coach.Id);
        _sessionRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedSession);

        await _handler.Handle(command, CancellationToken.None);

        capturedSession.Should().NotBeNull();
        capturedSession!.SessionCode.Should().StartWith("SES-");
    }
}
