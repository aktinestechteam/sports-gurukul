using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.UpdateTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.UnitTests.Features.TrainingProgramManagement;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.TrainingProgram;

public class UpdateTrainingProgramCommandHandlerTests
{
    private readonly Mock<ITrainingProgramRepository> _trainingProgramRepositoryMock;
    private readonly Mock<ISportRepository> _sportRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateTrainingProgramCommandHandler>> _loggerMock;
    private readonly UpdateTrainingProgramCommandHandler _handler;

    private readonly Guid _programId = Guid.NewGuid();
    private readonly Guid _academyId = Guid.NewGuid();
    private readonly Guid _sportId = Guid.NewGuid();

    public UpdateTrainingProgramCommandHandlerTests()
    {
        _trainingProgramRepositoryMock = new Mock<ITrainingProgramRepository>();
        _sportRepositoryMock = new Mock<ISportRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateTrainingProgramCommandHandler>>();
        _handler = new UpdateTrainingProgramCommandHandler(
            _trainingProgramRepositoryMock.Object,
            _sportRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCommand()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, academyId: _academyId, sportId: _sportId);
        var sport = TestHelpers.CreateTestSport(id: _sportId);
        var command = CreateValidCommand();

        SetupSuccessfulMocks(program, sport);

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

        var command = CreateValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training program not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SportNotFound()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, academyId: _academyId, sportId: _sportId);

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _sportRepositoryMock
            .Setup(r => r.GetByIdAsync(_sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Sport?)null);

        var command = CreateValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sport not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DuplicateNameInAcademy()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, academyId: _academyId, sportId: _sportId);
        var sport = TestHelpers.CreateTestSport(id: _sportId);
        var existingProgram = TestHelpers.CreateTestProgram(
            id: Guid.NewGuid(),
            academyId: _academyId,
            sportId: _sportId,
            programName: "Updated Program Name");

        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _sportRepositoryMock
            .Setup(r => r.GetByIdAsync(_sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);
        _trainingProgramRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(_academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.TrainingProgram> { existingProgram });

        var command = CreateValidCommand();
        command.ProgramName = "Updated Program Name";

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training program with this name already exists in this academy");
    }

    [Fact]
    public async Task Handle_Should_UpdateProgramFields_When_ValidCommand()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, academyId: _academyId, sportId: _sportId);
        var sport = TestHelpers.CreateTestSport(id: _sportId);
        var command = CreateValidCommand();

        SetupSuccessfulMocks(program, sport);

        await _handler.Handle(command, CancellationToken.None);

        program.ProgramName.Should().Be(command.ProgramName);
        program.SportId.Should().Be(command.SportId);
        program.Description.Should().Be(command.Description);
        program.DifficultyLevel.Should().Be(command.DifficultyLevel);
        program.MinimumAge.Should().Be(command.MinimumAge);
        program.MaximumAge.Should().Be(command.MaximumAge);
        program.DurationWeeks.Should().Be(command.DurationWeeks);
        program.Capacity.Should().Be(command.Capacity);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_When_Successful()
    {
        var program = TestHelpers.CreateTestProgram(id: _programId, academyId: _academyId, sportId: _sportId);
        var sport = TestHelpers.CreateTestSport(id: _sportId);
        var command = CreateValidCommand();

        SetupSuccessfulMocks(program, sport);

        await _handler.Handle(command, CancellationToken.None);

        _trainingProgramRepositoryMock.Verify(r => r.Update(program), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private UpdateTrainingProgramCommand CreateValidCommand() => new()
    {
        Id = _programId,
        ProgramName = "Updated Program Name",
        SportId = _sportId,
        Description = "Updated description",
        DifficultyLevel = DifficultyLevel.Intermediate,
        MinimumAge = 10,
        MaximumAge = 20,
        DurationWeeks = 16,
        Capacity = 50
    };

    private void SetupSuccessfulMocks(Domain.Entities.TrainingProgram program, Domain.Entities.Sport sport)
    {
        _trainingProgramRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        _sportRepositoryMock
            .Setup(r => r.GetByIdAsync(_sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);
        _trainingProgramRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(_academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.TrainingProgram>());
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }
}
