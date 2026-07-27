using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.DeleteTrainingProgram;
using SportsGurukul.Domain.Enums;
using SportsGurukul.UnitTests.Features.TrainingProgramManagement;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.TrainingProgram;

public class DeleteTrainingProgramCommandHandlerTests
{
    private readonly Mock<ITrainingProgramRepository> _trainingProgramRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteTrainingProgramCommandHandler>> _loggerMock;
    private readonly DeleteTrainingProgramCommandHandler _handler;

    private readonly Guid _programId = Guid.NewGuid();

    public DeleteTrainingProgramCommandHandlerTests()
    {
        _trainingProgramRepositoryMock = new Mock<ITrainingProgramRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteTrainingProgramCommandHandler>>();
        _handler = new DeleteTrainingProgramCommandHandler(
            _trainingProgramRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCommand()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, status: TrainingProgramStatus.Draft);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteTrainingProgramCommand { Id = _programId };

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

        var command = new DeleteTrainingProgramCommand { Id = _programId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training program not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProgramIsArchived()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, status: TrainingProgramStatus.Archived);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);

        var command = new DeleteTrainingProgramCommand { Id = _programId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot delete an archived training program");
    }

    [Fact]
    public async Task Handle_Should_SetArchivedStatus_When_Deleting()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, status: TrainingProgramStatus.Active);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteTrainingProgramCommand { Id = _programId };

        await _handler.Handle(command, CancellationToken.None);

        program.Status.Should().Be(TrainingProgramStatus.Archived);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_When_Successful()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, status: TrainingProgramStatus.Draft);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteTrainingProgramCommand { Id = _programId };

        await _handler.Handle(command, CancellationToken.None);

        _trainingProgramRepositoryMock.Verify(r => r.Update(program), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
