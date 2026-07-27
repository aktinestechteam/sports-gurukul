using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.Competition.Engines;
using SportsGurukul.Platform.Competition.Engines.Formats;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models.Enums;
using SportsGurukul.Platform.Competition.Seeding;
using SportsGurukul.Platform.Competition.Services;
using SportsGurukul.Platform.Competition.Services.Providers;

namespace SportsGurukul.Platform.Competition;

public static class DependencyInjection
{
    public static IServiceCollection AddCompetitionEngine(this IServiceCollection services)
    {
        services.AddSingleton<IFormatStrategy, SingleEliminationStrategy>();
        services.AddSingleton<IFormatStrategy, DoubleEliminationStrategy>();
        services.AddSingleton<IFormatStrategy, RoundRobinStrategy>();
        services.AddSingleton<IFormatStrategy, SwissSystemStrategy>();
        services.AddSingleton<IFormatStrategy, LeagueStrategy>();
        services.AddSingleton<IFormatStrategy, HybridTournamentStrategy>();
        services.AddSingleton<IFormatStrategy, GroupStageKnockoutStrategy>();

        services.AddSingleton<ISeedingStrategy, RandomSeedingStrategy>();
        services.AddSingleton<ISeedingStrategy, RankingBasedSeedingStrategy>();
        services.AddSingleton<ISeedingStrategy, ManualSeedingStrategy>();
        services.AddSingleton<ISeedingStrategy, RegionalSeedingStrategy>();
        services.AddSingleton<ISeedingStrategy, AcademyBasedSeedingStrategy>();
        services.AddSingleton<ISeedingStrategy, BalancedDrawSeedingStrategy>();

        services.AddScoped<IBracketGenerationService, BracketGenerationService>();
        services.AddScoped<IFixtureGenerationService, FixtureGenerationService>();
        services.AddScoped<ISeedingService, SeedingService>();
        services.AddScoped<IAdvancementService, AdvancementService>();
        services.AddScoped<IMatchAssignmentService, MatchAssignmentService>();
        services.AddScoped<IMatchScheduler, MatchScheduler>();
        services.AddScoped<IRankingCalculator, RankingCalculator>();
        services.AddScoped<ICompetitionEngine, CompetitionEngine>();

        services.AddSingleton<MemoryMatchStore>();
        services.AddSingleton<ILiveUpdatePublisher, SignalRLiveUpdatePublisher>();
        services.AddSingleton<ILiveScoreCache, RedisLiveScoreCache>();
        services.AddSingleton<ISportRuleProvider, FootballSportRuleProvider>();
        services.AddSingleton<ISportRuleProvider, CricketSportRuleProvider>();
        services.AddSingleton<ISportRuleProvider, BadmintonSportRuleProvider>();
        services.AddSingleton<ISportRuleProvider, ChessSportRuleProvider>();
        services.AddSingleton<ISportRuleProvider, AthleticsSportRuleProvider>();
        services.AddSingleton<ISportRuleProvider, SwimmingSportRuleProvider>();
        services.AddScoped<ILiveScoringService, LiveScoringService>();
        services.AddScoped<IMatchLifecycleService, MatchLifecycleService>();
        services.AddScoped<IRankingService, RankingService>();
        services.AddScoped<IStandingsService, StandingsService>();
        services.AddScoped<IMedalService, MedalService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<IStatisticsService, StatisticsService>();

        return services;
    }
}
