using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Tests.Common;

public static class TestMocks
{
    public static Mock<IUserRepository> CreateUserRepository() => new();
    public static Mock<IAthleteRepository> CreateAthleteRepository() => new();
    public static Mock<ISportRepository> CreateSportRepository() => new();
    public static Mock<IRepository<Domain.Entities.MedicalProfile>> CreateMedicalProfileRepository() => new();
    public static Mock<IRepository<Domain.Entities.EmergencyContact>> CreateEmergencyContactRepository() => new();
    public static Mock<IRepository<Domain.Entities.Ranking>> CreateRankingRepository() => new();
    public static Mock<IRepository<Domain.Entities.AthleteSport>> CreateAthleteSportRepository() => new();
    public static Mock<IRepository<Domain.Entities.Achievement>> CreateAchievementRepository() => new();
    public static Mock<IRepository<Domain.Entities.AthleteAchievement>> CreateAthleteAchievementRepository() => new();
    public static Mock<IUnitOfWork> CreateUnitOfWork() => new();
    public static Mock<ICacheService> CreateCacheService() => new();
    public static Mock<IRecentSearchRepository> CreateRecentSearchRepository() => new();
    public static Mock<ISavedSearchRepository> CreateSavedSearchRepository() => new();
    public static Mock<ICoachRepository> CreateCoachRepository() => new();
    public static Mock<ICoachAvailabilityRepository> CreateCoachAvailabilityRepository() => new();
    public static Mock<ICoachCertificationRepository> CreateCoachCertificationRepository() => new();
    public static Mock<IRepository<Domain.Entities.CoachSport>> CreateCoachSportRepository() => new();
    public static Mock<IRepository<Domain.Entities.CoachExperience>> CreateCoachExperienceRepository() => new();
    public static Mock<IRepository<Domain.Entities.CoachEducation>> CreateCoachEducationRepository() => new();
    public static Mock<IRepository<Domain.Entities.CoachLocation>> CreateCoachLocationRepository() => new();
    public static Mock<IRepository<Domain.Entities.CoachAthlete>> CreateCoachAthleteRepository() => new();
    public static Mock<ICoachSearchRepository> CreateCoachSearchRepository() => new();
    public static Mock<IBookingRepository> CreateBookingRepository() => new();
    public static Mock<IConflictRepository> CreateConflictRepository() => new();
    public static Mock<IWaitlistRepository> CreateWaitlistRepository() => new();

    public static Mock<ILogger<T>> CreateLogger<T>() where T : class => new();
}
