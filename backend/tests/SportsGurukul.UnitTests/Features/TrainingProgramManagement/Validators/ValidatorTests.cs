using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.TrainingProgramManagement.Validators;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.CreateTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.UpdateTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.DeleteTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.ArchiveTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.PublishTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.RestoreTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CreateTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.UpdateTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.StartTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CompleteTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CancelTrainingBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.AssignCoachToBatch;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CreateTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.UpdateTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CompleteTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CancelTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.RescheduleTrainingSession;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.AssignFacility;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.EnrollAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.TransferEnrollment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CompleteEnrollment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CancelEnrollment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.MarkAttendance;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.UpdateAttendance;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckInAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckOutAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.CreateAssessment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.SubmitAssessmentResult;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.PublishAssessmentResults;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.UpdateTrainingProgress;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.CompleteMilestone;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.IssueCertificate;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Validators;

public class ValidatorTests
{
    #region CreateTrainingProgramCommandValidator

    public class CreateTrainingProgramCommandValidatorTests
    {
        private readonly CreateTrainingProgramCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_AcademyId_Empty()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.Empty,
                SportId = Guid.NewGuid(),
                ProgramName = "Test",
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AcademyId);
        }

        [Fact]
        public void Should_Have_Error_When_SportId_Empty()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.NewGuid(),
                SportId = Guid.Empty,
                ProgramName = "Test",
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SportId);
        }

        [Fact]
        public void Should_Have_Error_When_ProgramName_Empty()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                ProgramName = string.Empty,
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ProgramName);
        }

        [Fact]
        public void Should_Have_Error_When_ProgramName_ExceedsMaxLength()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                ProgramName = new string('A', 201),
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ProgramName);
        }

        [Fact]
        public void Should_Have_Error_When_MinimumAge_OutOfRange()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                ProgramName = "Test",
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 3,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MinimumAge);
        }

        [Fact]
        public void Should_Have_Error_When_MaximumAge_OutOfRange()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                ProgramName = "Test",
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 150,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MaximumAge);
        }

        [Fact]
        public void Should_Have_Error_When_DurationWeeks_OutOfRange()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                ProgramName = "Test",
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 60,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DurationWeeks);
        }

        [Fact]
        public void Should_Have_Error_When_Capacity_OutOfRange()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                ProgramName = "Test",
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 0
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Capacity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CreateTrainingProgramCommand
            {
                AcademyId = Guid.NewGuid(),
                SportId = Guid.NewGuid(),
                ProgramName = "Test Program",
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region UpdateTrainingProgramCommandValidator

    public class UpdateTrainingProgramCommandValidatorTests
    {
        private readonly UpdateTrainingProgramCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new UpdateTrainingProgramCommand
            {
                Id = Guid.Empty,
                ProgramName = "Test",
                SportId = Guid.NewGuid(),
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_ProgramName_Empty()
        {
            var command = new UpdateTrainingProgramCommand
            {
                Id = Guid.NewGuid(),
                ProgramName = string.Empty,
                SportId = Guid.NewGuid(),
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ProgramName);
        }

        [Fact]
        public void Should_Have_Error_When_SportId_Empty()
        {
            var command = new UpdateTrainingProgramCommand
            {
                Id = Guid.NewGuid(),
                ProgramName = "Test",
                SportId = Guid.Empty,
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 8,
                MaximumAge = 16,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SportId);
        }

        [Fact]
        public void Should_Have_Error_When_MaximumAge_LessThan_MinimumAge()
        {
            var command = new UpdateTrainingProgramCommand
            {
                Id = Guid.NewGuid(),
                ProgramName = "Test",
                SportId = Guid.NewGuid(),
                DifficultyLevel = DifficultyLevel.Beginner,
                MinimumAge = 16,
                MaximumAge = 8,
                DurationWeeks = 12,
                Capacity = 30
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MaximumAge);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new UpdateTrainingProgramCommand
            {
                Id = Guid.NewGuid(),
                ProgramName = "Updated Program",
                SportId = Guid.NewGuid(),
                DifficultyLevel = DifficultyLevel.Intermediate,
                MinimumAge = 10,
                MaximumAge = 18,
                DurationWeeks = 16,
                Capacity = 25
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region DeleteTrainingProgramCommandValidator

    public class DeleteTrainingProgramCommandValidatorTests
    {
        private readonly DeleteTrainingProgramCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new DeleteTrainingProgramCommand { Id = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new DeleteTrainingProgramCommand { Id = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region ArchiveTrainingProgramCommandValidator

    public class ArchiveTrainingProgramCommandValidatorTests
    {
        private readonly ArchiveTrainingProgramCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new ArchiveTrainingProgramCommand { Id = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new ArchiveTrainingProgramCommand { Id = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region PublishTrainingProgramCommandValidator

    public class PublishTrainingProgramCommandValidatorTests
    {
        private readonly PublishTrainingProgramCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new PublishTrainingProgramCommand { Id = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new PublishTrainingProgramCommand { Id = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region RestoreTrainingProgramCommandValidator

    public class RestoreTrainingProgramCommandValidatorTests
    {
        private readonly RestoreTrainingProgramCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new RestoreTrainingProgramCommand { Id = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new RestoreTrainingProgramCommand { Id = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CreateTrainingBatchCommandValidator

    public class CreateTrainingBatchCommandValidatorTests
    {
        private readonly CreateTrainingBatchCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_ProgramId_Empty()
        {
            var command = new CreateTrainingBatchCommand(
                ProgramId: Guid.Empty,
                CoachId: Guid.NewGuid(),
                BranchId: Guid.NewGuid(),
                StartDate: DateTime.UtcNow.AddDays(7),
                EndDate: null,
                MaximumSeats: 30);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ProgramId);
        }

        [Fact]
        public void Should_Have_Error_When_CoachId_Empty()
        {
            var command = new CreateTrainingBatchCommand(
                ProgramId: Guid.NewGuid(),
                CoachId: Guid.Empty,
                BranchId: Guid.NewGuid(),
                StartDate: DateTime.UtcNow.AddDays(7),
                EndDate: null,
                MaximumSeats: 30);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CoachId);
        }

        [Fact]
        public void Should_Have_Error_When_BranchId_Empty()
        {
            var command = new CreateTrainingBatchCommand(
                ProgramId: Guid.NewGuid(),
                CoachId: Guid.NewGuid(),
                BranchId: Guid.Empty,
                StartDate: DateTime.UtcNow.AddDays(7),
                EndDate: null,
                MaximumSeats: 30);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.BranchId);
        }

        [Fact]
        public void Should_Have_Error_When_MaximumSeats_OutOfRange()
        {
            var command = new CreateTrainingBatchCommand(
                ProgramId: Guid.NewGuid(),
                CoachId: Guid.NewGuid(),
                BranchId: Guid.NewGuid(),
                StartDate: DateTime.UtcNow.AddDays(7),
                EndDate: null,
                MaximumSeats: 0);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MaximumSeats);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CreateTrainingBatchCommand(
                ProgramId: Guid.NewGuid(),
                CoachId: Guid.NewGuid(),
                BranchId: Guid.NewGuid(),
                StartDate: DateTime.UtcNow.AddDays(7),
                EndDate: DateTime.UtcNow.AddDays(90),
                MaximumSeats: 30);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region UpdateTrainingBatchCommandValidator

    public class UpdateTrainingBatchCommandValidatorTests
    {
        private readonly UpdateTrainingBatchCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new UpdateTrainingBatchCommand(
                Id: Guid.Empty,
                StartDate: DateTime.UtcNow.AddDays(1),
                EndDate: null,
                MaximumSeats: 30);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_MaximumSeats_OutOfRange()
        {
            var command = new UpdateTrainingBatchCommand(
                Id: Guid.NewGuid(),
                StartDate: DateTime.UtcNow.AddDays(1),
                EndDate: null,
                MaximumSeats: 0);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MaximumSeats);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new UpdateTrainingBatchCommand(
                Id: Guid.NewGuid(),
                StartDate: DateTime.UtcNow.AddDays(1),
                EndDate: DateTime.UtcNow.AddDays(90),
                MaximumSeats: 30);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region StartTrainingBatchCommandValidator

    public class StartTrainingBatchCommandValidatorTests
    {
        private readonly StartTrainingBatchCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new StartTrainingBatchCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new StartTrainingBatchCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CompleteTrainingBatchCommandValidator

    public class CompleteTrainingBatchCommandValidatorTests
    {
        private readonly CompleteTrainingBatchCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new CompleteTrainingBatchCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CompleteTrainingBatchCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CancelTrainingBatchCommandValidator

    public class CancelTrainingBatchCommandValidatorTests
    {
        private readonly CancelTrainingBatchCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new CancelTrainingBatchCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CancelTrainingBatchCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region AssignCoachToBatchCommandValidator

    public class AssignCoachToBatchCommandValidatorTests
    {
        private readonly AssignCoachToBatchCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new AssignCoachToBatchCommand(Guid.Empty, Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_CoachId_Empty()
        {
            var command = new AssignCoachToBatchCommand(Guid.NewGuid(), Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CoachId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new AssignCoachToBatchCommand(Guid.NewGuid(), Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CreateTrainingSessionCommandValidator

    public class CreateTrainingSessionCommandValidatorTests
    {
        private readonly CreateTrainingSessionCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_BatchId_Empty()
        {
            var command = new CreateTrainingSessionCommand(
                Guid.Empty, "Test Session", SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0),
                null, Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.BatchId);
        }

        [Fact]
        public void Should_Have_Error_When_SessionTitle_Empty()
        {
            var command = new CreateTrainingSessionCommand(
                Guid.NewGuid(), string.Empty, SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0),
                null, Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SessionTitle);
        }

        [Fact]
        public void Should_Have_Error_When_CoachId_Empty()
        {
            var command = new CreateTrainingSessionCommand(
                Guid.NewGuid(), "Test", SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0),
                null, Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CoachId);
        }

        [Fact]
        public void Should_Have_Error_When_EndTime_Before_StartTime()
        {
            var command = new CreateTrainingSessionCommand(
                Guid.NewGuid(), "Test", SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(11, 0, 0), new TimeSpan(9, 0, 0),
                null, Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CreateTrainingSessionCommand(
                Guid.NewGuid(), "Test Session", SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0),
                null, Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region UpdateTrainingSessionCommandValidator

    public class UpdateTrainingSessionCommandValidatorTests
    {
        private readonly UpdateTrainingSessionCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new UpdateTrainingSessionCommand(
                Guid.Empty, "Test", SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0));
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_SessionTitle_Empty()
        {
            var command = new UpdateTrainingSessionCommand(
                Guid.NewGuid(), string.Empty, SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0));
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SessionTitle);
        }

        [Fact]
        public void Should_Have_Error_When_EndTime_Before_StartTime()
        {
            var command = new UpdateTrainingSessionCommand(
                Guid.NewGuid(), "Test", SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(11, 0, 0), new TimeSpan(9, 0, 0));
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new UpdateTrainingSessionCommand(
                Guid.NewGuid(), "Test Session", SessionType.Practice,
                DateTime.UtcNow.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0));
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CompleteTrainingSessionCommandValidator

    public class CompleteTrainingSessionCommandValidatorTests
    {
        private readonly CompleteTrainingSessionCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new CompleteTrainingSessionCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CompleteTrainingSessionCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CancelTrainingSessionCommandValidator

    public class CancelTrainingSessionCommandValidatorTests
    {
        private readonly CancelTrainingSessionCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new CancelTrainingSessionCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CancelTrainingSessionCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region RescheduleTrainingSessionCommandValidator

    public class RescheduleTrainingSessionCommandValidatorTests
    {
        private readonly RescheduleTrainingSessionCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new RescheduleTrainingSessionCommand(
                Guid.Empty, DateTime.UtcNow.AddDays(1),
                new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0));
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_EndTime_Before_StartTime()
        {
            var command = new RescheduleTrainingSessionCommand(
                Guid.NewGuid(), DateTime.UtcNow.AddDays(1),
                new TimeSpan(11, 0, 0), new TimeSpan(9, 0, 0));
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new RescheduleTrainingSessionCommand(
                Guid.NewGuid(), DateTime.UtcNow.AddDays(1),
                new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0));
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region AssignFacilityCommandValidator

    public class AssignFacilityCommandValidatorTests
    {
        private readonly AssignFacilityCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Empty()
        {
            var command = new AssignFacilityCommand(Guid.Empty, Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_FacilityId_Null()
        {
            var command = new AssignFacilityCommand(Guid.NewGuid(), null);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FacilityId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new AssignFacilityCommand(Guid.NewGuid(), Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region EnrollAthleteCommandValidator

    public class EnrollAthleteCommandValidatorTests
    {
        private readonly EnrollAthleteCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_BatchId_Empty()
        {
            var command = new EnrollAthleteCommand { BatchId = Guid.Empty, AthleteId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.BatchId);
        }

        [Fact]
        public void Should_Have_Error_When_AthleteId_Empty()
        {
            var command = new EnrollAthleteCommand { BatchId = Guid.NewGuid(), AthleteId = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AthleteId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new EnrollAthleteCommand { BatchId = Guid.NewGuid(), AthleteId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region TransferEnrollmentCommandValidator

    public class TransferEnrollmentCommandValidatorTests
    {
        private readonly TransferEnrollmentCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_EnrollmentId_Empty()
        {
            var command = new TransferEnrollmentCommand
            {
                EnrollmentId = Guid.Empty,
                SourceBatchId = Guid.NewGuid(),
                TargetBatchId = Guid.NewGuid()
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EnrollmentId);
        }

        [Fact]
        public void Should_Have_Error_When_TargetBatchId_Empty()
        {
            var command = new TransferEnrollmentCommand
            {
                EnrollmentId = Guid.NewGuid(),
                SourceBatchId = Guid.NewGuid(),
                TargetBatchId = Guid.Empty
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.TargetBatchId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new TransferEnrollmentCommand
            {
                EnrollmentId = Guid.NewGuid(),
                SourceBatchId = Guid.NewGuid(),
                TargetBatchId = Guid.NewGuid()
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CompleteEnrollmentCommandValidator

    public class CompleteEnrollmentCommandValidatorTests
    {
        private readonly CompleteEnrollmentCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_EnrollmentId_Empty()
        {
            var command = new CompleteEnrollmentCommand { EnrollmentId = Guid.Empty, BatchId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EnrollmentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CompleteEnrollmentCommand { EnrollmentId = Guid.NewGuid(), BatchId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CancelEnrollmentCommandValidator

    public class CancelEnrollmentCommandValidatorTests
    {
        private readonly CancelEnrollmentCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_EnrollmentId_Empty()
        {
            var command = new CancelEnrollmentCommand { EnrollmentId = Guid.Empty, BatchId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EnrollmentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CancelEnrollmentCommand { EnrollmentId = Guid.NewGuid(), BatchId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region MarkAttendanceCommandValidator

    public class MarkAttendanceCommandValidatorTests
    {
        private readonly MarkAttendanceCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_SessionId_Empty()
        {
            var command = new MarkAttendanceCommand
            {
                SessionId = Guid.Empty,
                AthleteId = Guid.NewGuid(),
                Status = AttendanceStatus.Present
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SessionId);
        }

        [Fact]
        public void Should_Have_Error_When_AthleteId_Empty()
        {
            var command = new MarkAttendanceCommand
            {
                SessionId = Guid.NewGuid(),
                AthleteId = Guid.Empty,
                Status = AttendanceStatus.Present
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AthleteId);
        }

        [Fact]
        public void Should_Have_Error_When_Remarks_ExceedsMaxLength()
        {
            var command = new MarkAttendanceCommand
            {
                SessionId = Guid.NewGuid(),
                AthleteId = Guid.NewGuid(),
                Status = AttendanceStatus.Present,
                Remarks = new string('R', 501)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Remarks);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new MarkAttendanceCommand
            {
                SessionId = Guid.NewGuid(),
                AthleteId = Guid.NewGuid(),
                Status = AttendanceStatus.Present
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region UpdateAttendanceCommandValidator

    public class UpdateAttendanceCommandValidatorTests
    {
        private readonly UpdateAttendanceCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_AttendanceId_Empty()
        {
            var command = new UpdateAttendanceCommand
            {
                AttendanceId = Guid.Empty,
                Status = AttendanceStatus.Present
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AttendanceId);
        }

        [Fact]
        public void Should_Have_Error_When_Remarks_ExceedsMaxLength()
        {
            var command = new UpdateAttendanceCommand
            {
                AttendanceId = Guid.NewGuid(),
                Status = AttendanceStatus.Present,
                Remarks = new string('R', 501)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Remarks);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new UpdateAttendanceCommand
            {
                AttendanceId = Guid.NewGuid(),
                Status = AttendanceStatus.Present
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CheckInAthleteCommandValidator

    public class CheckInAthleteCommandValidatorTests
    {
        private readonly CheckInAthleteCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_SessionId_Empty()
        {
            var command = new CheckInAthleteCommand { SessionId = Guid.Empty, AthleteId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SessionId);
        }

        [Fact]
        public void Should_Have_Error_When_AthleteId_Empty()
        {
            var command = new CheckInAthleteCommand { SessionId = Guid.NewGuid(), AthleteId = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AthleteId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CheckInAthleteCommand { SessionId = Guid.NewGuid(), AthleteId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CheckOutAthleteCommandValidator

    public class CheckOutAthleteCommandValidatorTests
    {
        private readonly CheckOutAthleteCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_SessionId_Empty()
        {
            var command = new CheckOutAthleteCommand { SessionId = Guid.Empty, AthleteId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SessionId);
        }

        [Fact]
        public void Should_Have_Error_When_AthleteId_Empty()
        {
            var command = new CheckOutAthleteCommand { SessionId = Guid.NewGuid(), AthleteId = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AthleteId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CheckOutAthleteCommand { SessionId = Guid.NewGuid(), AthleteId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CreateAssessmentCommandValidator

    public class CreateAssessmentCommandValidatorTests
    {
        private readonly CreateAssessmentCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_SessionId_Empty()
        {
            var command = new CreateAssessmentCommand
            {
                SessionId = Guid.Empty,
                AssessmentType = "SkillTest",
                AssessmentName = "Test",
                MaximumScore = 100,
                PassingScore = 50
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.SessionId);
        }

        [Fact]
        public void Should_Have_Error_When_AssessmentName_Empty()
        {
            var command = new CreateAssessmentCommand
            {
                SessionId = Guid.NewGuid(),
                AssessmentType = "SkillTest",
                AssessmentName = string.Empty,
                MaximumScore = 100,
                PassingScore = 50
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AssessmentName);
        }

        [Fact]
        public void Should_Have_Error_When_MaximumScore_Zero()
        {
            var command = new CreateAssessmentCommand
            {
                SessionId = Guid.NewGuid(),
                AssessmentType = "SkillTest",
                AssessmentName = "Test",
                MaximumScore = 0,
                PassingScore = 0
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MaximumScore);
        }

        [Fact]
        public void Should_Have_Error_When_PassingScore_Exceeds_MaximumScore()
        {
            var command = new CreateAssessmentCommand
            {
                SessionId = Guid.NewGuid(),
                AssessmentType = "SkillTest",
                AssessmentName = "Test",
                MaximumScore = 100,
                PassingScore = 150
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PassingScore);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CreateAssessmentCommand
            {
                SessionId = Guid.NewGuid(),
                AssessmentType = "SkillTest",
                AssessmentName = "Mid-term",
                MaximumScore = 100,
                PassingScore = 50
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region SubmitAssessmentResultCommandValidator

    public class SubmitAssessmentResultCommandValidatorTests
    {
        private readonly SubmitAssessmentResultCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_AssessmentId_Empty()
        {
            var command = new SubmitAssessmentResultCommand
            {
                AssessmentId = Guid.Empty,
                AthleteId = Guid.NewGuid(),
                Score = 75
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AssessmentId);
        }

        [Fact]
        public void Should_Have_Error_When_AthleteId_Empty()
        {
            var command = new SubmitAssessmentResultCommand
            {
                AssessmentId = Guid.NewGuid(),
                AthleteId = Guid.Empty,
                Score = 75
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AthleteId);
        }

        [Fact]
        public void Should_Have_Error_When_Score_Negative()
        {
            var command = new SubmitAssessmentResultCommand
            {
                AssessmentId = Guid.NewGuid(),
                AthleteId = Guid.NewGuid(),
                Score = -5
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Score);
        }

        [Fact]
        public void Should_Have_Error_When_Remarks_ExceedsMaxLength()
        {
            var command = new SubmitAssessmentResultCommand
            {
                AssessmentId = Guid.NewGuid(),
                AthleteId = Guid.NewGuid(),
                Score = 75,
                Remarks = new string('R', 501)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Remarks);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new SubmitAssessmentResultCommand
            {
                AssessmentId = Guid.NewGuid(),
                AthleteId = Guid.NewGuid(),
                Score = 75,
                Remarks = "Good"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region PublishAssessmentResultsCommandValidator

    public class PublishAssessmentResultsCommandValidatorTests
    {
        private readonly PublishAssessmentResultsCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_AssessmentId_Empty()
        {
            var command = new PublishAssessmentResultsCommand { AssessmentId = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AssessmentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new PublishAssessmentResultsCommand { AssessmentId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region UpdateTrainingProgressCommandValidator

    public class UpdateTrainingProgressCommandValidatorTests
    {
        private readonly UpdateTrainingProgressCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_EnrollmentId_Empty()
        {
            var command = new UpdateTrainingProgressCommand
            {
                EnrollmentId = Guid.Empty,
                CurrentLevel = "Beginner",
                CompletedPercentage = 50
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EnrollmentId);
        }

        [Fact]
        public void Should_Have_Error_When_CurrentLevel_Empty()
        {
            var command = new UpdateTrainingProgressCommand
            {
                EnrollmentId = Guid.NewGuid(),
                CurrentLevel = string.Empty,
                CompletedPercentage = 50
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CurrentLevel);
        }

        [Fact]
        public void Should_Have_Error_When_CompletedPercentage_OutOfRange()
        {
            var command = new UpdateTrainingProgressCommand
            {
                EnrollmentId = Guid.NewGuid(),
                CurrentLevel = "Beginner",
                CompletedPercentage = 150
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CompletedPercentage);
        }

        [Fact]
        public void Should_Have_Error_When_OverallRating_OutOfRange()
        {
            var command = new UpdateTrainingProgressCommand
            {
                EnrollmentId = Guid.NewGuid(),
                CurrentLevel = "Beginner",
                CompletedPercentage = 50,
                OverallRating = 6
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.OverallRating);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new UpdateTrainingProgressCommand
            {
                EnrollmentId = Guid.NewGuid(),
                CurrentLevel = "Intermediate",
                CompletedPercentage = 65,
                OverallRating = 4.0m
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region CompleteMilestoneCommandValidator

    public class CompleteMilestoneCommandValidatorTests
    {
        private readonly CompleteMilestoneCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_MilestoneId_Empty()
        {
            var command = new CompleteMilestoneCommand { ProgramId = Guid.NewGuid(), MilestoneId = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MilestoneId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new CompleteMilestoneCommand { ProgramId = Guid.NewGuid(), MilestoneId = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region IssueCertificateCommandValidator

    public class IssueCertificateCommandValidatorTests
    {
        private readonly IssueCertificateCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_EnrollmentId_Empty()
        {
            var command = new IssueCertificateCommand
            {
                EnrollmentId = Guid.Empty,
                CertificateType = "Completion"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.EnrollmentId);
        }

        [Fact]
        public void Should_Have_Error_When_FileUrl_ExceedsMaxLength()
        {
            var command = new IssueCertificateCommand
            {
                EnrollmentId = Guid.NewGuid(),
                CertificateType = "Completion",
                FileUrl = new string('U', 501)
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FileUrl);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ValidCommand()
        {
            var command = new IssueCertificateCommand
            {
                EnrollmentId = Guid.NewGuid(),
                CertificateType = "Completion",
                FileUrl = "https://example.com/cert.pdf"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion
}
