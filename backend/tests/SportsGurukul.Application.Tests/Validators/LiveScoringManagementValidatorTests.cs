using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.CompleteMatch;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.GenerateLeaderboard;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.PauseMatch;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.PublishResults;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.RecordForfeit;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.RecordWalkover;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.ResumeMatch;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.StartMatch;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.UndoScore;
using SportsGurukul.Application.Features.LiveScoringManagement.Commands.UpdateLiveScore;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.Leaderboard;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.LiveScore;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.MatchStatistics;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.MedalTable;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.PlayerStatistics;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.TournamentStandings;
using SportsGurukul.Application.Features.LiveScoringManagement.Validators;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class LiveScoringManagementValidatorTests
{
    #region Command Validators

    [Fact]
    public async Task StartLiveMatchValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new StartLiveMatchCommandValidator();
        var command = new StartLiveMatchCommand { MatchId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task StartLiveMatchValidator_ValidCommand_ShouldNotHaveErrors()
    {
        var validator = new StartLiveMatchCommandValidator();
        var command = new StartLiveMatchCommand { MatchId = Guid.NewGuid() };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task PauseMatchValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new PauseMatchCommandValidator();
        var command = new PauseMatchCommand { MatchId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task ResumeMatchValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new ResumeMatchCommandValidator();
        var command = new ResumeMatchCommand { MatchId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task UndoScoreValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new UndoScoreCommandValidator();
        var command = new UndoScoreCommand { MatchId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task CompleteMatchValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new CompleteMatchCommandValidator();
        var command = new CompleteMatchCommand { MatchId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task UpdateLiveScoreValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new UpdateLiveScoreCommandValidator();
        var command = new UpdateLiveScoreCommand
        {
            MatchId = Guid.Empty,
            ParticipantId = Guid.NewGuid(),
            Points = 1,
            PeriodNumber = 1
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task UpdateLiveScoreValidator_EmptyParticipantId_ShouldHaveError()
    {
        var validator = new UpdateLiveScoreCommandValidator();
        var command = new UpdateLiveScoreCommand
        {
            MatchId = Guid.NewGuid(),
            ParticipantId = Guid.Empty,
            Points = 1,
            PeriodNumber = 1
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ParticipantId);
    }

    [Fact]
    public async Task UpdateLiveScoreValidator_NegativePoints_ShouldHaveError()
    {
        var validator = new UpdateLiveScoreCommandValidator();
        var command = new UpdateLiveScoreCommand
        {
            MatchId = Guid.NewGuid(),
            ParticipantId = Guid.NewGuid(),
            Points = -1,
            PeriodNumber = 1
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Points);
    }

    [Fact]
    public async Task UpdateLiveScoreValidator_ZeroPeriodNumber_ShouldHaveError()
    {
        var validator = new UpdateLiveScoreCommandValidator();
        var command = new UpdateLiveScoreCommand
        {
            MatchId = Guid.NewGuid(),
            ParticipantId = Guid.NewGuid(),
            Points = 1,
            PeriodNumber = 0
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PeriodNumber);
    }

    [Fact]
    public async Task UpdateLiveScoreValidator_ValidCommand_ShouldNotHaveErrors()
    {
        var validator = new UpdateLiveScoreCommandValidator();
        var command = new UpdateLiveScoreCommand
        {
            MatchId = Guid.NewGuid(),
            ParticipantId = Guid.NewGuid(),
            Points = 2,
            Unit = ScoringUnit.Point,
            PeriodNumber = 1,
            Description = "Goal scored"
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task RecordWalkoverValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new RecordWalkoverCommandValidator();
        var command = new RecordWalkoverCommand
        {
            MatchId = Guid.Empty,
            WinnerId = Guid.NewGuid(),
            WinnerName = "Team A"
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task RecordWalkoverValidator_EmptyWinnerId_ShouldHaveError()
    {
        var validator = new RecordWalkoverCommandValidator();
        var command = new RecordWalkoverCommand
        {
            MatchId = Guid.NewGuid(),
            WinnerId = Guid.Empty,
            WinnerName = "Team A"
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.WinnerId);
    }

    [Fact]
    public async Task RecordWalkoverValidator_EmptyWinnerName_ShouldHaveError()
    {
        var validator = new RecordWalkoverCommandValidator();
        var command = new RecordWalkoverCommand
        {
            MatchId = Guid.NewGuid(),
            WinnerId = Guid.NewGuid(),
            WinnerName = string.Empty
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.WinnerName);
    }

    [Fact]
    public async Task RecordForfeitValidator_EmptyWinnerName_ShouldHaveError()
    {
        var validator = new RecordForfeitCommandValidator();
        var command = new RecordForfeitCommand
        {
            MatchId = Guid.NewGuid(),
            WinnerId = Guid.NewGuid(),
            WinnerName = string.Empty
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.WinnerName);
    }

    [Fact]
    public async Task PublishResultsValidator_EmptyTournamentId_ShouldHaveError()
    {
        var validator = new PublishResultsCommandValidator();
        var command = new PublishResultsCommand
        {
            TournamentId = Guid.Empty,
            MatchId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.TournamentId);
    }

    [Fact]
    public async Task PublishResultsValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new PublishResultsCommandValidator();
        var command = new PublishResultsCommand
        {
            TournamentId = Guid.NewGuid(),
            MatchId = Guid.Empty
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task GenerateLeaderboardValidator_EmptyTournamentId_ShouldHaveError()
    {
        var validator = new GenerateLeaderboardCommandValidator();
        var command = new GenerateLeaderboardCommand
        {
            TournamentId = Guid.Empty,
            Type = LeaderboardType.Tournament
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.TournamentId);
    }

    [Fact]
    public async Task GenerateLeaderboardValidator_ValidCommand_ShouldNotHaveErrors()
    {
        var validator = new GenerateLeaderboardCommandValidator();
        var command = new GenerateLeaderboardCommand
        {
            TournamentId = Guid.NewGuid(),
            Type = LeaderboardType.Tournament,
            SportCode = "football"
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Query Validators

    [Fact]
    public async Task LiveScoreQueryValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new LiveScoreQueryValidator();
        var query = new LiveScoreQuery { MatchId = Guid.Empty };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task LeaderboardQueryValidator_EmptyTournamentId_ShouldHaveError()
    {
        var validator = new LeaderboardQueryValidator();
        var query = new LeaderboardQuery
        {
            TournamentId = Guid.Empty,
            Type = LeaderboardType.Tournament
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.TournamentId);
    }

    [Fact]
    public async Task TournamentStandingsQueryValidator_EmptyTournamentId_ShouldHaveError()
    {
        var validator = new TournamentStandingsQueryValidator();
        var query = new TournamentStandingsQuery { TournamentId = Guid.Empty };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.TournamentId);
    }

    [Fact]
    public async Task MedalTableQueryValidator_EmptyTournamentId_ShouldHaveError()
    {
        var validator = new MedalTableQueryValidator();
        var query = new MedalTableQuery { TournamentId = Guid.Empty };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.TournamentId);
    }

    [Fact]
    public async Task MatchStatisticsQueryValidator_EmptyMatchId_ShouldHaveError()
    {
        var validator = new MatchStatisticsQueryValidator();
        var query = new MatchStatisticsQuery { MatchId = Guid.Empty };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public async Task PlayerStatisticsQueryValidator_EmptyTournamentId_ShouldHaveError()
    {
        var validator = new PlayerStatisticsQueryValidator();
        var query = new PlayerStatisticsQuery
        {
            TournamentId = Guid.Empty,
            PlayerId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.TournamentId);
    }

    [Fact]
    public async Task PlayerStatisticsQueryValidator_EmptyPlayerId_ShouldHaveError()
    {
        var validator = new PlayerStatisticsQueryValidator();
        var query = new PlayerStatisticsQuery
        {
            TournamentId = Guid.NewGuid(),
            PlayerId = Guid.Empty
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PlayerId);
    }

    #endregion
}
