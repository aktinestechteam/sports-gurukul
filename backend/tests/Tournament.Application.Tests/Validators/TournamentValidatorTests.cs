using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;
using SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateTournament;
using SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateScore;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RegisterParticipant;
using SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateRankings;
using SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateFixtures;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CompleteMatch;
using SportsGurukul.Application.Features.TournamentManagement.Queries.SearchTournaments;
using SportsGurukul.Application.Features.TournamentManagement.Validators;

namespace Tournament.Application.Tests.Validators;

public class TournamentValidatorTests
{
    private static CreateTournamentCommand CreateValidCreateCommand() => new()
    {
        TournamentName = "Summer Championship",
        AcademyId = Guid.NewGuid(),
        SportId = Guid.NewGuid(),
        StartDate = DateTime.UtcNow.AddDays(30),
        EndDate = DateTime.UtcNow.AddDays(37),
        RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
        RegistrationCloseDate = DateTime.UtcNow.AddDays(25)
    };

    #region CreateTournamentCommandValidator

    public class CreateTournamentCommandValidatorTests
    {
        private readonly CreateTournamentCommandValidator _validator = new();

        [Fact]
        public void TournamentName_WhenEmpty_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.TournamentName = string.Empty;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.TournamentName));
        }

        [Fact]
        public void TournamentName_WhenNull_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.TournamentName = null!;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.TournamentName));
        }

        [Fact]
        public void TournamentName_WhenExceeds200Characters_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.TournamentName = new string('A', 201);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.TournamentName));
        }

        [Fact]
        public void TournamentName_WhenExactly200Characters_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.TournamentName = new string('A', 200);

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.TournamentName));
        }

        [Fact]
        public void TournamentName_WhenValid_ShouldPass()
        {
            var command = CreateValidCreateCommand();

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.TournamentName));
        }

        [Fact]
        public void AcademyId_WhenEmpty_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.AcademyId = Guid.Empty;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.AcademyId));
        }

        [Fact]
        public void AcademyId_WhenValid_ShouldPass()
        {
            var command = CreateValidCreateCommand();

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.AcademyId));
        }

        [Fact]
        public void SportId_WhenEmpty_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.SportId = Guid.Empty;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.SportId));
        }

        [Fact]
        public void SportId_WhenValid_ShouldPass()
        {
            var command = CreateValidCreateCommand();

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.SportId));
        }

        [Fact]
        public void StartDate_WhenInPast_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.StartDate = DateTime.UtcNow.AddDays(-1);
            command.EndDate = DateTime.UtcNow.AddDays(6);
            command.RegistrationCloseDate = DateTime.UtcNow.AddDays(-5);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.StartDate));
        }

        [Fact]
        public void StartDate_WhenDefault_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.StartDate = default;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void StartDate_WhenInFuture_ShouldPass()
        {
            var command = CreateValidCreateCommand();

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.StartDate));
        }

        [Fact]
        public void EndDate_WhenBeforeStartDate_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            var start = DateTime.UtcNow.AddDays(30);
            command.StartDate = start;
            command.EndDate = start.AddDays(-1);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.EndDate));
        }

        [Fact]
        public void EndDate_WhenEqualToStartDate_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            var start = DateTime.UtcNow.AddDays(30);
            command.StartDate = start;
            command.EndDate = start;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.EndDate));
        }

        [Fact]
        public void EndDate_WhenAfterStartDate_ShouldPass()
        {
            var command = CreateValidCreateCommand();

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.EndDate));
        }

        [Fact]
        public void RegistrationCloseDate_WhenAfterStartDate_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            var start = DateTime.UtcNow.AddDays(30);
            command.StartDate = start;
            command.RegistrationCloseDate = start.AddDays(5);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.RegistrationCloseDate));
        }

        [Fact]
        public void RegistrationCloseDate_WhenEqualToStartDate_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            var start = DateTime.UtcNow.AddDays(30);
            command.StartDate = start;
            command.RegistrationCloseDate = start;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.RegistrationCloseDate));
        }

        [Fact]
        public void RegistrationCloseDate_WhenBeforeStartDate_ShouldPass()
        {
            var command = CreateValidCreateCommand();

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.RegistrationCloseDate));
        }

        [Fact]
        public void MaxParticipants_WhenZero_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.MaxParticipants = 0;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.MaxParticipants));
        }

        [Fact]
        public void MaxParticipants_WhenNegative_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.MaxParticipants = -5;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.MaxParticipants));
        }

        [Fact]
        public void MaxParticipants_WhenPositive_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.MaxParticipants = 64;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.MaxParticipants));
        }

        [Fact]
        public void MaxParticipants_WhenNull_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.MaxParticipants = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.MaxParticipants));
        }

        [Fact]
        public void MinParticipants_WhenZero_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.MinParticipants = 0;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.MinParticipants));
        }

        [Fact]
        public void MinParticipants_WhenNegative_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.MinParticipants = -1;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.MinParticipants));
        }

        [Fact]
        public void MinParticipants_WhenPositive_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.MinParticipants = 4;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.MinParticipants));
        }

        [Fact]
        public void MinParticipants_WhenNull_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.MinParticipants = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.MinParticipants));
        }

        [Fact]
        public void RegistrationFee_WhenNegative_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.RegistrationFee = -10;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.RegistrationFee));
        }

        [Fact]
        public void RegistrationFee_WhenZero_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.RegistrationFee = 0;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.RegistrationFee));
        }

        [Fact]
        public void RegistrationFee_WhenPositive_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.RegistrationFee = 150.00m;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.RegistrationFee));
        }

        [Fact]
        public void RegistrationFee_WhenNull_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.RegistrationFee = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.RegistrationFee));
        }

        [Fact]
        public void Description_WhenExceeds2000Characters_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.Description = new string('A', 2001);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.Description));
        }

        [Fact]
        public void Description_WhenExactly2000Characters_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.Description = new string('A', 2000);

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.Description));
        }

        [Fact]
        public void Description_WhenNull_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.Description = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.Description));
        }

        [Fact]
        public void ContactEmail_WhenInvalidFormat_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.ContactEmail = "not-an-email";

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.ContactEmail));
        }

        [Fact]
        public void ContactEmail_WhenValid_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.ContactEmail = "contact@tournament.com";

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.ContactEmail));
        }

        [Fact]
        public void ContactEmail_WhenNull_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.ContactEmail = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.ContactEmail));
        }

        [Fact]
        public void ContactEmail_WhenEmpty_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.ContactEmail = string.Empty;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.ContactEmail));
        }

        [Fact]
        public void ContactPhone_WhenExceeds20Characters_ShouldFail()
        {
            var command = CreateValidCreateCommand();
            command.ContactPhone = new string('1', 21);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTournamentCommand.ContactPhone));
        }

        [Fact]
        public void ContactPhone_WhenExactly20Characters_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.ContactPhone = new string('1', 20);

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.ContactPhone));
        }

        [Fact]
        public void ContactPhone_WhenNull_ShouldPass()
        {
            var command = CreateValidCreateCommand();
            command.ContactPhone = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateTournamentCommand.ContactPhone));
        }

        [Fact]
        public void ValidCommand_ShouldPassAllRules()
        {
            var command = CreateValidCreateCommand();

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }

    #endregion

    #region UpdateTournamentCommandValidator

    public class UpdateTournamentCommandValidatorTests
    {
        private readonly UpdateTournamentCommandValidator _validator = new();

        private static UpdateTournamentCommand CreateValidUpdateCommand() => new()
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "Updated Tournament"
        };

        [Fact]
        public void TournamentId_WhenEmpty_ShouldFail()
        {
            var command = CreateValidUpdateCommand();
            command.TournamentId = Guid.Empty;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateTournamentCommand.TournamentId));
        }

        [Fact]
        public void TournamentId_WhenValid_ShouldPass()
        {
            var command = CreateValidUpdateCommand();

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.TournamentId));
        }

        [Fact]
        public void TournamentName_WhenExceeds200Characters_ShouldFail()
        {
            var command = CreateValidUpdateCommand();
            command.TournamentName = new string('A', 201);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateTournamentCommand.TournamentName));
        }

        [Fact]
        public void TournamentName_WhenNull_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.TournamentName = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.TournamentName));
        }

        [Fact]
        public void TournamentName_WhenEmpty_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.TournamentName = string.Empty;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.TournamentName));
        }

        [Fact]
        public void Description_WhenExceeds2000Characters_ShouldFail()
        {
            var command = CreateValidUpdateCommand();
            command.Description = new string('A', 2001);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateTournamentCommand.Description));
        }

        [Fact]
        public void Description_WhenWithinLimit_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.Description = new string('A', 2000);

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.Description));
        }

        [Fact]
        public void EndDate_WhenBeforeStartDate_ShouldFail()
        {
            var command = CreateValidUpdateCommand();
            var start = DateTime.UtcNow.AddDays(30);
            command.StartDate = start;
            command.EndDate = start.AddDays(-1);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateTournamentCommand.EndDate));
        }

        [Fact]
        public void EndDate_WhenBothNull_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.StartDate = null;
            command.EndDate = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.EndDate));
        }

        [Fact]
        public void EndDate_WhenOnlyOneSet_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.StartDate = DateTime.UtcNow.AddDays(30);
            command.EndDate = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.EndDate));
        }

        [Fact]
        public void EndDate_WhenAfterStartDate_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            var start = DateTime.UtcNow.AddDays(30);
            command.StartDate = start;
            command.EndDate = start.AddDays(7);

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.EndDate));
        }

        [Fact]
        public void MaxParticipants_WhenZero_ShouldFail()
        {
            var command = CreateValidUpdateCommand();
            command.MaxParticipants = 0;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateTournamentCommand.MaxParticipants));
        }

        [Fact]
        public void MaxParticipants_WhenNull_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.MaxParticipants = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.MaxParticipants));
        }

        [Fact]
        public void MaxParticipants_WhenPositive_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.MaxParticipants = 100;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.MaxParticipants));
        }

        [Fact]
        public void RegistrationFee_WhenNegative_ShouldFail()
        {
            var command = CreateValidUpdateCommand();
            command.RegistrationFee = -50;

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateTournamentCommand.RegistrationFee));
        }

        [Fact]
        public void RegistrationFee_WhenZero_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.RegistrationFee = 0;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.RegistrationFee));
        }

        [Fact]
        public void RegistrationFee_WhenNull_ShouldPass()
        {
            var command = CreateValidUpdateCommand();
            command.RegistrationFee = null;

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateTournamentCommand.RegistrationFee));
        }

        [Fact]
        public void ValidCommand_ShouldPassAllRules()
        {
            var command = CreateValidUpdateCommand();

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }

    #endregion

    #region UpdateScoreCommandValidator

    public class UpdateScoreCommandValidatorTests
    {
        private readonly UpdateScoreCommandValidator _validator = new();

        [Fact]
        public void MatchId_WhenEmpty_ShouldFail()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.Empty,
                HomeScore = 3,
                AwayScore = 2
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateScoreCommand.MatchId));
        }

        [Fact]
        public void MatchId_WhenValid_ShouldPass()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.NewGuid(),
                HomeScore = 3,
                AwayScore = 2
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateScoreCommand.MatchId));
        }

        [Fact]
        public void HomeScore_WhenNegative_ShouldFail()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.NewGuid(),
                HomeScore = -1,
                AwayScore = 0
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateScoreCommand.HomeScore));
        }

        [Fact]
        public void HomeScore_WhenZero_ShouldPass()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.NewGuid(),
                HomeScore = 0,
                AwayScore = 0
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateScoreCommand.HomeScore));
        }

        [Fact]
        public void HomeScore_WhenPositive_ShouldPass()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.NewGuid(),
                HomeScore = 10,
                AwayScore = 5
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateScoreCommand.HomeScore));
        }

        [Fact]
        public void AwayScore_WhenNegative_ShouldFail()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.NewGuid(),
                HomeScore = 0,
                AwayScore = -1
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateScoreCommand.AwayScore));
        }

        [Fact]
        public void AwayScore_WhenZero_ShouldPass()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.NewGuid(),
                HomeScore = 0,
                AwayScore = 0
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateScoreCommand.AwayScore));
        }

        [Fact]
        public void AwayScore_WhenPositive_ShouldPass()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.NewGuid(),
                HomeScore = 3,
                AwayScore = 7
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateScoreCommand.AwayScore));
        }

        [Fact]
        public void ValidCommand_ShouldPassAllRules()
        {
            var command = new UpdateScoreCommand
            {
                MatchId = Guid.NewGuid(),
                HomeScore = 3,
                AwayScore = 2
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }

    #endregion

    #region RegisterParticipantCommandValidator

    public class RegisterParticipantCommandValidatorTests
    {
        private readonly RegisterParticipantCommandValidator _validator = new();

        [Fact]
        public void TournamentId_WhenEmpty_ShouldFail()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.Empty,
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterParticipantCommand.TournamentId));
        }

        [Fact]
        public void TournamentId_WhenValid_ShouldPass()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.TournamentId));
        }

        [Fact]
        public void RegistrantName_WhenEmpty_ShouldFail()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = string.Empty,
                ParticipantType = TournamentParticipantType.Athlete
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterParticipantCommand.RegistrantName));
        }

        [Fact]
        public void RegistrantName_WhenNull_ShouldFail()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = null!,
                ParticipantType = TournamentParticipantType.Athlete
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterParticipantCommand.RegistrantName));
        }

        [Fact]
        public void RegistrantName_WhenExceeds200Characters_ShouldFail()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = new string('A', 201),
                ParticipantType = TournamentParticipantType.Athlete
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterParticipantCommand.RegistrantName));
        }

        [Fact]
        public void RegistrantName_WhenValid_ShouldPass()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.RegistrantName));
        }

        [Fact]
        public void Email_WhenInvalidFormat_ShouldFail()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                Email = "not-an-email"
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterParticipantCommand.Email));
        }

        [Fact]
        public void Email_WhenValid_ShouldPass()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                Email = "player@example.com"
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.Email));
        }

        [Fact]
        public void Email_WhenNull_ShouldPass()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                Email = null
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.Email));
        }

        [Fact]
        public void Phone_WhenExceeds20Characters_ShouldFail()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                Phone = new string('1', 21)
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterParticipantCommand.Phone));
        }

        [Fact]
        public void Phone_WhenWithinLimit_ShouldPass()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                Phone = "+1234567890"
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.Phone));
        }

        [Fact]
        public void AthleteId_WhenParticipantTypeIsAthleteAndEmpty_ShouldFail()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                AthleteId = null
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterParticipantCommand.AthleteId));
        }

        [Fact]
        public void AthleteId_WhenParticipantTypeIsAthleteAndValid_ShouldPass()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                AthleteId = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.AthleteId));
        }

        [Fact]
        public void AthleteId_WhenParticipantTypeIsTeam_ShouldNotRequire()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Team 1",
                ParticipantType = TournamentParticipantType.Team,
                AthleteId = Guid.Empty,
                TeamId = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.AthleteId));
        }

        [Fact]
        public void TeamId_WhenParticipantTypeIsTeamAndEmpty_ShouldFail()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Team 1",
                ParticipantType = TournamentParticipantType.Team,
                TeamId = null
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterParticipantCommand.TeamId));
        }

        [Fact]
        public void TeamId_WhenParticipantTypeIsTeamAndValid_ShouldPass()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Team 1",
                ParticipantType = TournamentParticipantType.Team,
                TeamId = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.TeamId));
        }

        [Fact]
        public void TeamId_WhenParticipantTypeIsAthlete_ShouldNotRequire()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                AthleteId = Guid.NewGuid(),
                TeamId = Guid.Empty
            };

            var result = _validator.Validate(command);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(RegisterParticipantCommand.TeamId));
        }

        [Fact]
        public void ValidAthleteCommand_ShouldPassAllRules()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Player 1",
                ParticipantType = TournamentParticipantType.Athlete,
                AthleteId = Guid.NewGuid(),
                Email = "player@example.com",
                Phone = "+1234567890"
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ValidTeamCommand_ShouldPassAllRules()
        {
            var command = new RegisterParticipantCommand
            {
                TournamentId = Guid.NewGuid(),
                RegistrantName = "Team 1",
                ParticipantType = TournamentParticipantType.Team,
                TeamId = Guid.NewGuid(),
                Email = "team@example.com"
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }

    #endregion

    #region GenerateRankingsCommandValidator

    public class GenerateRankingsCommandValidatorTests
    {
        private readonly GenerateRankingsCommandValidator _validator = new();

        [Fact]
        public void TournamentId_WhenEmpty_ShouldFail()
        {
            var command = new GenerateRankingsCommand
            {
                TournamentId = Guid.Empty
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GenerateRankingsCommand.TournamentId));
        }

        [Fact]
        public void TournamentId_WhenValid_ShouldPass()
        {
            var command = new GenerateRankingsCommand
            {
                TournamentId = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }

    #endregion

    #region GenerateFixturesCommandValidator

    public class GenerateFixturesCommandValidatorTests
    {
        private readonly GenerateFixturesCommandValidator _validator = new();

        [Fact]
        public void TournamentId_WhenEmpty_ShouldFail()
        {
            var command = new GenerateFixturesCommand
            {
                TournamentId = Guid.Empty
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GenerateFixturesCommand.TournamentId));
        }

        [Fact]
        public void TournamentId_WhenValid_ShouldPass()
        {
            var command = new GenerateFixturesCommand
            {
                TournamentId = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }

    #endregion

    #region CompleteMatchCommandValidator

    public class CompleteMatchCommandValidatorTests
    {
        private readonly CompleteMatchCommandValidator _validator = new();

        [Fact]
        public void MatchId_WhenEmpty_ShouldFail()
        {
            var command = new CompleteMatchCommand
            {
                MatchId = Guid.Empty
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CompleteMatchCommand.MatchId));
        }

        [Fact]
        public void MatchId_WhenValid_ShouldPass()
        {
            var command = new CompleteMatchCommand
            {
                MatchId = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }

    #endregion

    #region SearchTournamentsQueryValidator

    public class SearchTournamentsQueryValidatorTests
    {
        private readonly SearchTournamentsQueryValidator _validator = new();

        [Fact]
        public void Page_WhenZero_ShouldFail()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 0,
                PageSize = 20
            };

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchTournamentsQuery.Page));
        }

        [Fact]
        public void Page_WhenNegative_ShouldFail()
        {
            var query = new SearchTournamentsQuery
            {
                Page = -1,
                PageSize = 20
            };

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchTournamentsQuery.Page));
        }

        [Fact]
        public void Page_WhenOne_ShouldPass()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 1,
                PageSize = 20
            };

            var result = _validator.Validate(query);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(SearchTournamentsQuery.Page));
        }

        [Fact]
        public void PageSize_WhenZero_ShouldFail()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 1,
                PageSize = 0
            };

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchTournamentsQuery.PageSize));
        }

        [Fact]
        public void PageSize_WhenNegative_ShouldFail()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 1,
                PageSize = -5
            };

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchTournamentsQuery.PageSize));
        }

        [Fact]
        public void PageSize_WhenExceeds100_ShouldFail()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 1,
                PageSize = 101
            };

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchTournamentsQuery.PageSize));
        }

        [Fact]
        public void PageSize_WhenExactly1_ShouldPass()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 1,
                PageSize = 1
            };

            var result = _validator.Validate(query);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(SearchTournamentsQuery.PageSize));
        }

        [Fact]
        public void PageSize_WhenExactly100_ShouldPass()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 1,
                PageSize = 100
            };

            var result = _validator.Validate(query);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(SearchTournamentsQuery.PageSize));
        }

        [Fact]
        public void PageSize_WhenWithinRange_ShouldPass()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 1,
                PageSize = 50
            };

            var result = _validator.Validate(query);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(SearchTournamentsQuery.PageSize));
        }

        [Fact]
        public void ValidQuery_ShouldPassAllRules()
        {
            var query = new SearchTournamentsQuery
            {
                Page = 1,
                PageSize = 20
            };

            var result = _validator.Validate(query);

            result.IsValid.Should().BeTrue();
        }
    }

    #endregion
}
