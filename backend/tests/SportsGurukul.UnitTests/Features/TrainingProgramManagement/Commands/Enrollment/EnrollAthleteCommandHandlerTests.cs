using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.EnrollAthlete;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Enrollment;

public class EnrollAthleteCommandHandlerTests
{
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock;
    private readonly Mock<ILogger<EnrollAthleteCommandHandler>> _loggerMock;
    private readonly EnrollAthleteCommandHandler _handler;

    public EnrollAthleteCommandHandlerTests()
    {
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _athleteRepositoryMock = new Mock<IAthleteRepository>();
        _loggerMock = new Mock<ILogger<EnrollAthleteCommandHandler>>();

        _handler = new EnrollAthleteCommandHandler(
            _batchRepositoryMock.Object,
            _athleteRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static EnrollAthleteCommand CreateValidCommand(Guid? batchId = null, Guid? athleteId = null) => new()
    {
        BatchId = batchId ?? Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid()
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidEnrollment()
    {
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active, maximumSeats: 30);
        batch.Enrollments = new List<TrainingEnrollment>();
        var athlete = TestHelpers.CreateTestAthlete(id: athleteId);
        var command = CreateValidCommand(batchId, athleteId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BatchId.Should().Be(batchId);
        result.Value.AthleteId.Should().Be(athleteId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchNotFound()
    {
        var command = CreateValidCommand();

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Batch not found");
        _athleteRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchIsNotActive()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Inactive);
        batch.Enrollments = new List<TrainingEnrollment>();
        var command = CreateValidCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Batch is not active");
        _athleteRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AthleteNotFound()
    {
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);
        batch.Enrollments = new List<TrainingEnrollment>();
        var command = CreateValidCommand(batchId, athleteId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AthleteAlreadyEnrolled()
    {
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);
        var existingEnrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        batch.Enrollments = new List<TrainingEnrollment> { existingEnrollment };
        var command = CreateValidCommand(batchId, athleteId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestAthlete(athleteId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete is already enrolled in this batch");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchIsFull()
    {
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active, maximumSeats: 1);
        batch.Enrollments = new List<TrainingEnrollment>
        {
            TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: Guid.NewGuid(), status: EnrollmentStatus.Active)
        };
        var command = CreateValidCommand(batchId, athleteId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestAthlete(athleteId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Batch has reached maximum capacity");
    }

    [Fact]
    public async Task Handle_Should_SetActiveStatus_When_Enrolling()
    {
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active, maximumSeats: 30);
        batch.Enrollments = new List<TrainingEnrollment>();
        var athlete = TestHelpers.CreateTestAthlete(id: athleteId);
        var command = CreateValidCommand(batchId, athleteId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        batch.Enrollments.Should().HaveCount(1);
        batch.Enrollments.First().Status.Should().Be(EnrollmentStatus.Active);
        batch.Enrollments.First().AthleteId.Should().Be(athleteId);
        batch.Enrollments.First().BatchId.Should().Be(batchId);
    }
}
