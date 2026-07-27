using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CreateTrainingBatch;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using TrainingProgramEntity = SportsGurukul.Domain.Entities.TrainingProgram;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Batch;

public class CreateTrainingBatchCommandHandlerTests
{
    private readonly Mock<ILogger<CreateTrainingBatchCommandHandler>> _loggerMock;
    private readonly Mock<ITrainingProgramRepository> _programRepositoryMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ICoachRepository> _coachRepositoryMock;
    private readonly Mock<IAcademyBranchRepository> _branchRepositoryMock;
    private readonly CreateTrainingBatchCommandHandler _handler;

    public CreateTrainingBatchCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateTrainingBatchCommandHandler>>();
        _programRepositoryMock = new Mock<ITrainingProgramRepository>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _coachRepositoryMock = new Mock<ICoachRepository>();
        _branchRepositoryMock = new Mock<IAcademyBranchRepository>();

        _handler = new CreateTrainingBatchCommandHandler(
            _loggerMock.Object,
            _programRepositoryMock.Object,
            _batchRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _branchRepositoryMock.Object);
    }

    private static CreateTrainingBatchCommand CreateValidCommand(
        Guid? programId = null,
        Guid? coachId = null,
        Guid? branchId = null) => new(
        ProgramId: programId ?? Guid.NewGuid(),
        CoachId: coachId ?? Guid.NewGuid(),
        BranchId: branchId ?? Guid.NewGuid(),
        StartDate: DateTime.UtcNow.AddDays(1),
        EndDate: DateTime.UtcNow.AddDays(90),
        MaximumSeats: 30);

    private void SetupValidDependencies(CreateTrainingBatchCommand command, TrainingBatch batch)
    {
        var program = TestHelpers.CreateTestProgram(id: command.ProgramId);
        var coach = TestHelpers.CreateTestCoach(id: command.CoachId);
        var branch = TestHelpers.CreateTestBranch(id: command.BranchId);

        _programRepositoryMock.Setup(r => r.GetByIdAsync(command.ProgramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _branchRepositoryMock.Setup(r => r.GetByIdAsync(command.BranchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _batchRepositoryMock.Setup(r => r.IsBatchCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _batchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingBatch>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCommand()
    {
        var command = CreateValidCommand();
        var batch = TestHelpers.CreateTestBatch(programId: command.ProgramId, coachId: command.CoachId, branchId: command.BranchId);
        SetupValidDependencies(command, batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BatchCode.Should().Be(batch.BatchCode);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProgramNotFound()
    {
        var command = CreateValidCommand();

        _programRepositoryMock.Setup(r => r.GetByIdAsync(command.ProgramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgramEntity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Training program with ID {command.ProgramId} not found");
        _coachRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _batchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingBatch>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CoachNotFound()
    {
        var command = CreateValidCommand();
        var program = TestHelpers.CreateTestProgram(id: command.ProgramId);

        _programRepositoryMock.Setup(r => r.GetByIdAsync(command.ProgramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Coach with ID {command.CoachId} not found");
        _branchRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _batchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingBatch>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BranchNotFound()
    {
        var command = CreateValidCommand();
        var program = TestHelpers.CreateTestProgram(id: command.ProgramId);
        var coach = TestHelpers.CreateTestCoach(id: command.CoachId);

        _programRepositoryMock.Setup(r => r.GetByIdAsync(command.ProgramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _branchRepositoryMock.Setup(r => r.GetByIdAsync(command.BranchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyBranch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Academy branch with ID {command.BranchId} not found");
        _batchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingBatch>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_StartDateAfterEndDate()
    {
        var command = new CreateTrainingBatchCommand(
            ProgramId: Guid.NewGuid(),
            CoachId: Guid.NewGuid(),
            BranchId: Guid.NewGuid(),
            StartDate: DateTime.UtcNow.AddDays(90),
            EndDate: DateTime.UtcNow.AddDays(1),
            MaximumSeats: 30);

        var program = TestHelpers.CreateTestProgram(id: command.ProgramId);
        var coach = TestHelpers.CreateTestCoach(id: command.CoachId);
        var branch = TestHelpers.CreateTestBranch(id: command.BranchId);

        _programRepositoryMock.Setup(r => r.GetByIdAsync(command.ProgramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _branchRepositoryMock.Setup(r => r.GetByIdAsync(command.BranchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Start date must be before end date");
        _batchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingBatch>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_SetWaitlistedStatus_When_Creating()
    {
        var command = CreateValidCommand();
        TrainingBatch? capturedBatch = null;

        var program = TestHelpers.CreateTestProgram(id: command.ProgramId);
        var coach = TestHelpers.CreateTestCoach(id: command.CoachId);
        var branch = TestHelpers.CreateTestBranch(id: command.BranchId);
        var createdBatch = TestHelpers.CreateTestBatch(programId: command.ProgramId, coachId: command.CoachId, branchId: command.BranchId);

        _programRepositoryMock.Setup(r => r.GetByIdAsync(command.ProgramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _branchRepositoryMock.Setup(r => r.GetByIdAsync(command.BranchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _batchRepositoryMock.Setup(r => r.IsBatchCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _batchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingBatch>(), It.IsAny<CancellationToken>()))
            .Callback<TrainingBatch, CancellationToken>((b, _) => capturedBatch = b)
            .ReturnsAsync(createdBatch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBatch);

        await _handler.Handle(command, CancellationToken.None);

        capturedBatch.Should().NotBeNull();
        capturedBatch!.Status.Should().Be(BatchStatus.Waitlisted);
    }

    [Fact]
    public async Task Handle_Should_GenerateBatchCode_When_Creating()
    {
        var command = CreateValidCommand();
        TrainingBatch? capturedBatch = null;

        var program = TestHelpers.CreateTestProgram(id: command.ProgramId);
        var coach = TestHelpers.CreateTestCoach(id: command.CoachId);
        var branch = TestHelpers.CreateTestBranch(id: command.BranchId);
        var createdBatch = TestHelpers.CreateTestBatch(programId: command.ProgramId, coachId: command.CoachId, branchId: command.BranchId);

        _programRepositoryMock.Setup(r => r.GetByIdAsync(command.ProgramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _branchRepositoryMock.Setup(r => r.GetByIdAsync(command.BranchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
        _batchRepositoryMock.Setup(r => r.IsBatchCodeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _batchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingBatch>(), It.IsAny<CancellationToken>()))
            .Callback<TrainingBatch, CancellationToken>((b, _) => capturedBatch = b)
            .ReturnsAsync(createdBatch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBatch);

        await _handler.Handle(command, CancellationToken.None);

        capturedBatch.Should().NotBeNull();
        capturedBatch!.BatchCode.Should().StartWith("BAT-");
    }
}
