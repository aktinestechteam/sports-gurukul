using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CancelEnrollment;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Enrollment;

public class CancelEnrollmentCommandHandlerTests
{
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ILogger<CancelEnrollmentCommandHandler>> _loggerMock;
    private readonly CancelEnrollmentCommandHandler _handler;

    public CancelEnrollmentCommandHandlerTests()
    {
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _loggerMock = new Mock<ILogger<CancelEnrollmentCommandHandler>>();

        _handler = new CancelEnrollmentCommandHandler(
            _batchRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static CancelEnrollmentCommand CreateValidCommand(Guid? enrollmentId = null, Guid? batchId = null) => new()
    {
        EnrollmentId = enrollmentId ?? Guid.NewGuid(),
        BatchId = batchId ?? Guid.NewGuid()
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCancellation()
    {
        var batchId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: batchId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var command = CreateValidCommand(enrollmentId, batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Status.Should().Be("Withdrawn");
        _batchRepositoryMock.Verify(r => r.Update(batch), Times.Once);
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
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentNotFound()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment>();
        var command = CreateValidCommand(batchId: batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Enrollment not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentIsNotActive()
    {
        var batchId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: batchId, status: EnrollmentStatus.Withdrawn);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var command = CreateValidCommand(enrollmentId, batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only active enrollments can be cancelled");
    }
}
