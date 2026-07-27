using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace TournamentTestShared;

public static class TestDataBuilder
{
    public static Tournament CreateTournament(
        TournamentStatus status = TournamentStatus.Draft,
        int? maxParticipants = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(30);
        var end = endDate ?? start.AddDays(7);
        return new Tournament
        {
            Id = Guid.NewGuid(),
            TournamentCode = $"TRN-{DateTime.UtcNow:yyyyMMddHHmmss}",
            TournamentName = "Test Tournament",
            Description = "Test Description",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            TournamentType = TournamentType.League,
            Status = status,
            StartDate = start,
            EndDate = end,
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
            MaxParticipants = maxParticipants,
            MinParticipants = 4,
            RegistrationFee = 100.00m,
            RegistrationType = RegistrationType.Individual,
            Venue = "Test Venue",
            Rules = "Test Rules",
            ContactEmail = "test@example.com",
            ContactPhone = "+1234567890",
            IsPublished = status != TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            RowVersion = []
        };
    }

    public static TournamentMatch CreateMatch(
        MatchStatus status = MatchStatus.Scheduled,
        Guid? tournamentId = null)
    {
        return new TournamentMatch
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId ?? Guid.NewGuid(),
            TournamentStageId = Guid.NewGuid(),
            TournamentRoundId = Guid.NewGuid(),
            MatchNumber = 1,
            HomeParticipantId = Guid.NewGuid(),
            HomeParticipantName = "Team A",
            AwayParticipantId = Guid.NewGuid(),
            AwayParticipantName = "Team B",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            ScheduledTime = new TimeSpan(14, 0, 0),
            Status = status,
            HomeScore = 0,
            AwayScore = 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static TournamentRegistration CreateRegistration(
        TournamentRegistrationStatus status = TournamentRegistrationStatus.Pending,
        Guid? tournamentId = null,
        Guid? athleteId = null)
    {
        return new TournamentRegistration
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId ?? Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            RegistrationStatus = status,
            AthleteId = athleteId ?? Guid.NewGuid(),
            TeamId = null,
            AcademyId = Guid.NewGuid(),
            RegistrantName = "Test Registrant",
            Email = "registrant@test.com",
            Phone = "+1234567890",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static TournamentParticipant CreateParticipant(
        bool isActive = true,
        Guid? tournamentId = null)
    {
        return new TournamentParticipant
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId ?? Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = Guid.NewGuid(),
            AcademyId = Guid.NewGuid(),
            ParticipantName = "Test Participant",
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static TournamentRanking CreateRanking(
        int rank = 1,
        Guid? tournamentId = null,
        Guid? participantId = null)
    {
        return new TournamentRanking
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId ?? Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            ParticipantId = participantId ?? Guid.NewGuid(),
            Rank = rank,
            Points = 100 - (rank * 10),
            Wins = 5 - rank,
            Losses = rank - 1,
            Draws = 0,
            MatchesPlayed = 5,
            SetsWon = 15 - (rank * 3),
            SetsLost = rank * 3,
            GamesWon = 50 - (rank * 10),
            GamesLost = rank * 10,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static TournamentFixture CreateFixture(Guid? tournamentId = null)
    {
        return new TournamentFixture
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId ?? Guid.NewGuid(),
            TournamentStageId = Guid.NewGuid(),
            FixtureNumber = 1,
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            ScheduledTime = new TimeSpan(14, 0, 0),
            HomeTeamName = "Team A",
            AwayTeamName = "Team B",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static TournamentStage CreateStage(Guid? tournamentId = null)
    {
        return new TournamentStage
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId ?? Guid.NewGuid(),
            StageName = "Group Stage",
            StageOrder = 1,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static TournamentResult CreateResult(Guid? tournamentId = null)
    {
        return new TournamentResult
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId ?? Guid.NewGuid(),
            MatchId = Guid.NewGuid(),
            WinnerId = Guid.NewGuid(),
            WinnerName = "Team A",
            HomeScore = 3,
            AwayScore = 1,
            ResultDetails = "Final Score",
            IsVerified = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static TournamentAward CreateAward(
        TournamentAwardType awardType = TournamentAwardType.Winner,
        Guid? tournamentId = null)
    {
        return new TournamentAward
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId ?? Guid.NewGuid(),
            AwardType = awardType,
            AwardName = awardType.ToString(),
            ParticipantId = Guid.NewGuid(),
            Description = $"Rank finish",
            CreatedAt = DateTime.UtcNow
        };
    }
}
