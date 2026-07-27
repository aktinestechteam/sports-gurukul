using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.CreateTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using DomainEntities = SportsGurukul.Domain.Entities;
using DomainEnums = SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.TrainingProgram;

public class CreateTrainingProgramCommandHandlerTests
{
    private readonly Mock<ITrainingProgramRepository> _programRepositoryMock = new();
    private readonly Mock<IAcademyRepository> _academyRepositoryMock = new();
    private readonly Mock<ISportRepository> _sportRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<CreateTrainingProgramCommandHandler>> _loggerMock = new();
    private readonly CreateTrainingProgramCommandHandler _handler;

    public CreateTrainingProgramCommandHandlerTests()
    {
        _handler = new CreateTrainingProgramCommandHandler(
            _programRepositoryMock.Object,
            _academyRepositoryMock.Object,
            _sportRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    private static CreateTrainingProgramCommand CreateValidCommand(
        Guid? academyId = null,
        Guid? sportId = null) => new()
    {
        ProgramName = "New Cricket Program",
        SportId = sportId ?? Guid.NewGuid(),
        AcademyId = academyId ?? Guid.NewGuid(),
        Description = "A comprehensive cricket training program",
        DifficultyLevel = DomainEnums.DifficultyLevel.Intermediate,
        MinimumAge = 10,
        MaximumAge = 16,
        DurationWeeks = 16,
        Capacity = 25
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCommand()
    {
        var academy = TestHelpers.CreateTestAcademy();
        var sport = TestHelpers.CreateTestSport();
        var command = CreateValidCommand(academyId: academy.Id, sportId: sport.Id);

        _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(academy);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(sport);
        _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<DomainEntities.TrainingProgram>());
        _programRepositoryMock.Setup(r => r.AddAsync(It.IsAny<DomainEntities.TrainingProgram>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((DomainEntities.TrainingProgram p, System.Threading.CancellationToken _) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.ProgramName.Should().Be(command.ProgramName);
        result.Value.AcademyId.Should().Be(command.AcademyId);
        result.Value.SportId.Should().Be(command.SportId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AcademyNotFound()
    {
        var command = CreateValidCommand();

        _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((DomainEntities.Academy?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found");
        _sportRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
        _programRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DomainEntities.TrainingProgram>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SportNotFound()
    {
        var academy = TestHelpers.CreateTestAcademy();
        var command = CreateValidCommand(academyId: academy.Id);

        _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(academy);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((DomainEntities.Sport?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sport not found");
        _programRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DomainEntities.TrainingProgram>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DuplicateNameInAcademy()
    {
        var academy = TestHelpers.CreateTestAcademy();
        var sport = TestHelpers.CreateTestSport();
        var command = CreateValidCommand(academyId: academy.Id, sportId: sport.Id);

        _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(academy);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(sport);
        _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<DomainEntities.TrainingProgram>
            {
                TestHelpers.CreateTestProgram(programName: "New Cricket Program")
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training program with this name already exists in this academy");
        _programRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DomainEntities.TrainingProgram>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_GenerateProgramCode_When_Creating()
    {
        var academy = TestHelpers.CreateTestAcademy();
        var sport = TestHelpers.CreateTestSport();
        var command = CreateValidCommand(academyId: academy.Id, sportId: sport.Id);

        _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(academy);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(sport);
        _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<DomainEntities.TrainingProgram>());

        DomainEntities.TrainingProgram? capturedProgram = null;
        _programRepositoryMock.Setup(r => r.AddAsync(It.IsAny<DomainEntities.TrainingProgram>(), It.IsAny<System.Threading.CancellationToken>()))
            .Callback<DomainEntities.TrainingProgram, System.Threading.CancellationToken>((p, _) => capturedProgram = p)
            .ReturnsAsync((DomainEntities.TrainingProgram p, System.Threading.CancellationToken _) => p);

        await _handler.Handle(command, CancellationToken.None);

        capturedProgram.Should().NotBeNull();
        capturedProgram!.ProgramCode.Should().StartWith("TPR-");
    }

    [Fact]
    public async Task Handle_Should_SetDraftStatus_When_Creating()
    {
        var academy = TestHelpers.CreateTestAcademy();
        var sport = TestHelpers.CreateTestSport();
        var command = CreateValidCommand(academyId: academy.Id, sportId: sport.Id);

        _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(academy);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(sport);
        _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<DomainEntities.TrainingProgram>());

        DomainEntities.TrainingProgram? capturedProgram = null;
        _programRepositoryMock.Setup(r => r.AddAsync(It.IsAny<DomainEntities.TrainingProgram>(), It.IsAny<System.Threading.CancellationToken>()))
            .Callback<DomainEntities.TrainingProgram, System.Threading.CancellationToken>((p, _) => capturedProgram = p)
            .ReturnsAsync((DomainEntities.TrainingProgram p, System.Threading.CancellationToken _) => p);

        await _handler.Handle(command, CancellationToken.None);

        capturedProgram.Should().NotBeNull();
        capturedProgram!.Status.Should().Be(DomainEnums.TrainingProgramStatus.Draft);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_When_Successful()
    {
        var academy = TestHelpers.CreateTestAcademy();
        var sport = TestHelpers.CreateTestSport();
        var command = CreateValidCommand(academyId: academy.Id, sportId: sport.Id);

        _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(academy);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(sport);
        _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<DomainEntities.TrainingProgram>());
        _programRepositoryMock.Setup(r => r.AddAsync(It.IsAny<DomainEntities.TrainingProgram>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((DomainEntities.TrainingProgram p, System.Threading.CancellationToken _) => p);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_SetSportNameAndAcademyName_When_ReturningDto()
    {
        var academy = TestHelpers.CreateTestAcademy();
        var sport = TestHelpers.CreateTestSport();
        var command = CreateValidCommand(academyId: academy.Id, sportId: sport.Id);

        _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(academy);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(sport);
        _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new List<DomainEntities.TrainingProgram>());
        _programRepositoryMock.Setup(r => r.AddAsync(It.IsAny<DomainEntities.TrainingProgram>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((DomainEntities.TrainingProgram p, System.Threading.CancellationToken _) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AcademyName.Should().Be(academy.Name);
        result.Value.SportName.Should().Be(sport.Name);
    }
}
