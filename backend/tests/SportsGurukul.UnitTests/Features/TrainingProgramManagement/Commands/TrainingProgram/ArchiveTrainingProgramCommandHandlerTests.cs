using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.ArchiveTrainingProgram;
using SportsGurukul.Domain.Enums;
using SportsGurukul.UnitTests.Features.TrainingProgramManagement;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.TrainingProgram;

public class ArchiveTrainingProgramCommandHandlerTests
{
    private readonly Mock<ITrainingProgramRepository> _trainingProgramRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ArchiveTrainingProgramCommandHandler>> _loggerMock;
    private readonly ArchiveTrainingProgramCommandHandler _handler;

    private readonly Guid _programId = Guid.NewGuid();

    public ArchiveTrainingProgramCommandHandlerTests()
    {
        _trainingProgramRepositoryMock = new Mock<ITrainingProgramRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ArchiveTrainingProgramCommandHandler>>();
        _handler = new ArchiveTrainingProgramCommandHandler(
            _trainingProgramRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ProgramIsActive()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, status: TrainingProgramStatus.Active);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new ArchiveTrainingProgramCommand { Id = _programId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ProgramIsCompleted()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, status: TrainingProgramStatus.Completed);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new ArchiveTrainingProgramCommand { Id = _programId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProgramNotFound()
    {
        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.TrainingProgram?)null);

        var command = new ArchiveTrainingProgramCommand { Id = _programId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training program not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProgramIsDraft()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, status: TrainingProgramStatus.Draft);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);

        var command = new ArchiveTrainingProgramCommand { Id = _programId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training program can only be archived when Active or Completed");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProgramIsAlreadyArchived()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, status: TrainingProgramStatus.Archived);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);

        var command = new ArchiveTrainingProgramCommand { Id = _programId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training program can only be archived when Active or Completed");
    }
}
