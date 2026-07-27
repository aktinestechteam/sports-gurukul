using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.CreateTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.DeleteTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.UpdateTrainingProgram;
using TrainingProgramEntity = SportsGurukul.Domain.Entities.TrainingProgram;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.BusinessRules;

public class BusinessRuleTests
{
    #region ProgramCode Generation

    public class ProgramCodeGenerationTests
    {
        private readonly Mock<ITrainingProgramRepository> _programRepositoryMock;
        private readonly Mock<IAcademyRepository> _academyRepositoryMock;
        private readonly Mock<ISportRepository> _sportRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateTrainingProgramCommandHandler>> _loggerMock;
        private readonly CreateTrainingProgramCommandHandler _handler;

        public ProgramCodeGenerationTests()
        {
            _programRepositoryMock = new Mock<ITrainingProgramRepository>();
            _academyRepositoryMock = new Mock<IAcademyRepository>();
            _sportRepositoryMock = new Mock<ISportRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateTrainingProgramCommandHandler>>();
            _handler = new CreateTrainingProgramCommandHandler(
                _programRepositoryMock.Object,
                _academyRepositoryMock.Object,
                _sportRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_GenerateProgramCode_MatchingPattern()
        {
            var academy = TestHelpers.CreateTestAcademy();
            var sport = TestHelpers.CreateTestSport();

            _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(academy);
            _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sport);
            _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingProgramEntity>());

            TrainingProgramEntity? capturedProgram = null;
            _programRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingProgramEntity>(), It.IsAny<CancellationToken>()))
                .Callback<TrainingProgramEntity, CancellationToken>((p, _) => capturedProgram = p)
                .ReturnsAsync((TrainingProgramEntity p, CancellationToken _) => p);

            var command = new CreateTrainingProgramCommand
            {
                AcademyId = academy.Id,
                SportId = sport.Id,
                ProgramName = "Test Program",
                DifficultyLevel = Domain.Enums.DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };

            await _handler.Handle(command, CancellationToken.None);

            capturedProgram.Should().NotBeNull();
            var pattern = @"^TPR-\d{8}-[A-Z0-9]{6}$";
            Regex.IsMatch(capturedProgram!.ProgramCode, pattern).Should().BeTrue(
                $"ProgramCode '{capturedProgram.ProgramCode}' should match pattern TPR-{{yyyyMMdd}}-{{6 alphanumeric chars}}");
        }
    }

    #endregion

    #region BatchCode Generation

    public class BatchCodeGenerationTests
    {
        [Fact]
        public void BatchCode_Should_MatchExpectedPattern()
        {
            var batchCode = "BAT-20260726-123456";
            var pattern = @"^BAT-\d{8}-\d{6}$";
            Regex.IsMatch(batchCode, pattern).Should().BeTrue(
                $"BatchCode '{batchCode}' should match pattern BAT-{{yyyyMMdd}}-{{6 digits}}");
        }

        [Fact]
        public void BatchCode_Should_Contain_DateAndDigits()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var batchCode = $"BAT-{datePart}-123456";

            batchCode.Should().StartWith("BAT-");
            batchCode.Should().Contain(datePart);
        }
    }

    #endregion

    #region SessionCode Generation

    public class SessionCodeGenerationTests
    {
        [Fact]
        public void SessionCode_Should_MatchExpectedPattern()
        {
            var sessionCode = "SES-20260726-123456";
            var pattern = @"^SES-\d{8}-\d{6}$";
            Regex.IsMatch(sessionCode, pattern).Should().BeTrue(
                $"SessionCode '{sessionCode}' should match pattern SES-{{yyyyMMdd}}-{{6 digits}}");
        }

        [Fact]
        public void SessionCode_Should_Contain_DateAndDigits()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var sessionCode = $"SES-{datePart}-123456";

            sessionCode.Should().StartWith("SES-");
            sessionCode.Should().Contain(datePart);
        }
    }

    #endregion

    #region Soft Delete

    public class SoftDeleteTests
    {
        private readonly Mock<ITrainingProgramRepository> _programRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DeleteTrainingProgramCommandHandler>> _loggerMock;
        private readonly DeleteTrainingProgramCommandHandler _handler;

        public SoftDeleteTests()
        {
            _programRepositoryMock = new Mock<ITrainingProgramRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeleteTrainingProgramCommandHandler>>();
            _handler = new DeleteTrainingProgramCommandHandler(
                _programRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_SetArchivedStatus_When_Deleting()
        {
            var program = TestHelpers.CreateTestProgram(status: Domain.Enums.TrainingProgramStatus.Active);

            _programRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(program.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(program);

            var command = new DeleteTrainingProgramCommand { Id = program.Id };
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            program.Status.Should().Be(Domain.Enums.TrainingProgramStatus.Archived);
        }

        [Fact]
        public async Task Handle_Should_NotRemoveEntity_When_Deleting()
        {
            var program = TestHelpers.CreateTestProgram(status: Domain.Enums.TrainingProgramStatus.Active);

            _programRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(program.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(program);

            var command = new DeleteTrainingProgramCommand { Id = program.Id };
            await _handler.Handle(command, CancellationToken.None);

            _programRepositoryMock.Verify(r => r.Remove(It.IsAny<TrainingProgramEntity>()), Times.Never);
        }
    }

    #endregion

    #region Audit Fields

    public class AuditFieldsTests
    {
        private readonly Mock<ITrainingProgramRepository> _programRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdateTrainingProgramCommandHandler>> _updateLoggerMock;
        private readonly Mock<ILogger<CreateTrainingProgramCommandHandler>> _createLoggerMock;
        private readonly Mock<IAcademyRepository> _academyRepositoryMock;
        private readonly Mock<ISportRepository> _sportRepositoryMock;

        public AuditFieldsTests()
        {
            _programRepositoryMock = new Mock<ITrainingProgramRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _updateLoggerMock = new Mock<ILogger<UpdateTrainingProgramCommandHandler>>();
            _createLoggerMock = new Mock<ILogger<CreateTrainingProgramCommandHandler>>();
            _academyRepositoryMock = new Mock<IAcademyRepository>();
            _sportRepositoryMock = new Mock<ISportRepository>();
        }

        [Fact]
        public async Task Create_Should_PersistProgram_When_ValidCommand()
        {
            var handler = new CreateTrainingProgramCommandHandler(
                _programRepositoryMock.Object,
                _academyRepositoryMock.Object,
                _sportRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _createLoggerMock.Object);

            var academy = TestHelpers.CreateTestAcademy();
            var sport = TestHelpers.CreateTestSport();

            _academyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(academy);
            _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(sport);
            _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingProgramEntity>());

            TrainingProgramEntity? capturedProgram = null;
            _programRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingProgramEntity>(), It.IsAny<CancellationToken>()))
                .Callback<TrainingProgramEntity, CancellationToken>((p, _) => capturedProgram = p)
                .ReturnsAsync((TrainingProgramEntity p, CancellationToken _) => p);

            var command = new CreateTrainingProgramCommand
            {
                AcademyId = academy.Id,
                SportId = sport.Id,
                ProgramName = "Test",
                DifficultyLevel = Domain.Enums.DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };

            await handler.Handle(command, CancellationToken.None);

            capturedProgram.Should().NotBeNull();
            capturedProgram!.ProgramName.Should().Be("Test");
            capturedProgram.Status.Should().Be(Domain.Enums.TrainingProgramStatus.Draft);
            capturedProgram.AcademyId.Should().Be(academy.Id);
            capturedProgram.SportId.Should().Be(sport.Id);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Should_ModifyAndPersistProgram_When_ValidCommand()
        {
            var handler = new UpdateTrainingProgramCommandHandler(
                _programRepositoryMock.Object,
                _sportRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _updateLoggerMock.Object);

            var program = TestHelpers.CreateTestProgram();
            var sport = TestHelpers.CreateTestSport();

            _programRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(program);
            _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(sport);
            _programRepositoryMock.Setup(r => r.GetByAcademyIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingProgramEntity>());

            var command = new UpdateTrainingProgramCommand
            {
                Id = program.Id,
                ProgramName = "Updated Name",
                SportId = sport.Id,
                DifficultyLevel = Domain.Enums.DifficultyLevel.Intermediate,
                MinimumAge = 10,
                MaximumAge = 18,
                DurationWeeks = 16,
                Capacity = 25
            };

            await handler.Handle(command, CancellationToken.None);

            program.ProgramName.Should().Be("Updated Name");
            program.SportId.Should().Be(sport.Id);
            program.DifficultyLevel.Should().Be(Domain.Enums.DifficultyLevel.Intermediate);
            program.MinimumAge.Should().Be(10);
            program.MaximumAge.Should().Be(18);
            program.DurationWeeks.Should().Be(16);
            program.Capacity.Should().Be(25);
            _programRepositoryMock.Verify(r => r.Update(program), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    #endregion
}
