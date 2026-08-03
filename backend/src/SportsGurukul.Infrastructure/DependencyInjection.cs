using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Infrastructure.Caching;
using SportsGurukul.Infrastructure.Email;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Infrastructure.Persistence.Repositories;
using SportsGurukul.Infrastructure.Persistence.Repositories.AI;
using SportsGurukul.Infrastructure.Persistence.Repositories.Finance;
using SportsGurukul.Infrastructure.Persistence.Repositories.Notification;
using SportsGurukul.Infrastructure.Storage;
using NNotificationAuditRepository = SportsGurukul.Infrastructure.Persistence.Repositories.Notification.AuditRepository;
using NNotificationIAuditRepository = SportsGurukul.Application.Common.Interfaces.Notification.IAuditRepository;
using AIAuditRepository = SportsGurukul.Infrastructure.Persistence.Repositories.AI.AuditRepository;
using IAIAuditRepository = SportsGurukul.Application.Common.Interfaces.AI.IAuditRepository;

namespace SportsGurukul.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Default"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IAthleteRepository, AthleteRepository>();
        services.AddScoped<ISportRepository, SportRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IAthleteDocumentRepository, AthleteDocumentRepository>();
        services.AddScoped<ISavedSearchRepository, SavedSearchRepository>();
        services.AddScoped<IRecentSearchRepository, RecentSearchRepository>();
        services.AddScoped<ICoachRepository, CoachRepository>();
        services.AddScoped<ICoachCertificationRepository, CoachCertificationRepository>();
        services.AddScoped<ICoachAvailabilityRepository, CoachAvailabilityRepository>();
        services.AddScoped<ICoachDocumentRepository, CoachDocumentRepository>();
        services.AddScoped<ICoachSearchRepository, CoachSearchRepository>();
        services.AddScoped<IAcademyRepository, AcademyRepository>();
        services.AddScoped<IAcademyBranchRepository, AcademyBranchRepository>();
        services.AddScoped<IAcademyFacilityRepository, AcademyFacilityRepository>();
        services.AddScoped<IAcademyMembershipRepository, AcademyMembershipRepository>();
        services.AddScoped<ICoachAcademyRepository, CoachAcademyRepository>();
        services.AddScoped<IAthleteAcademyRepository, AthleteAcademyRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<IFacilityCourtRepository, FacilityCourtRepository>();
        services.AddScoped<IFacilityEquipmentRepository, FacilityEquipmentRepository>();
        services.AddScoped<IFacilityScheduleRepository, FacilityScheduleRepository>();
        services.AddScoped<IFacilityPricingRepository, FacilityPricingRepository>();
        services.AddScoped<IAcademySearchRepository, AcademySearchRepository>();
        services.AddScoped<ITrainingProgramRepository, TrainingProgramRepository>();
        services.AddScoped<ITrainingBatchRepository, TrainingBatchRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<ITrainingProgressRepository, TrainingProgressRepository>();

        // Booking & Scheduling
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingScheduleRepository, BookingScheduleRepository>();
        services.AddScoped<IConflictRepository, ConflictRepository>();
        services.AddScoped<IWaitlistRepository, WaitlistRepository>();

        // Tournament Management
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<IBracketRepository, BracketRepository>();
        services.AddScoped<IRankingRepository, RankingRepository>();

        // Event Management
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventSearchRepository, EventSearchRepository>();
        services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();
        services.AddScoped<IEventAttendanceRepository, EventAttendanceRepository>();
        services.AddScoped<IEventFeedbackRepository, EventFeedbackRepository>();

        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddScoped<ICacheService, SearchCacheService>();

        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

        services.AddScoped<IFileStorageService>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<StorageOptions>>().Value;
            return options.Provider switch
            {
                StorageProvider.Azure => sp.GetRequiredService<AzureBlobStorageService>(),
                StorageProvider.S3 => sp.GetRequiredService<S3StorageService>(),
                _ => sp.GetRequiredService<LocalStorageService>()
            };
        });
        services.AddScoped<LocalStorageService>();
        services.AddScoped<AzureBlobStorageService>();
        services.AddScoped<S3StorageService>();

        services.AddScoped<IEmailService, SmtpEmailService>();

        // Finance Domain
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IRefundRepository, RefundRepository>();
        services.AddScoped<ILedgerRepository, LedgerRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();

        // Notification Domain
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IPreferenceRepository, PreferenceRepository>();
        services.AddScoped<IQueueRepository, QueueRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<NNotificationIAuditRepository, NNotificationAuditRepository>();

        // AI & Intelligence Domain
        services.AddScoped<IAIProviderRepository, AIProviderRepository>();
        services.AddScoped<IAssistantRepository, AssistantRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IPromptRepository, PromptRepository>();
        services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IEmbeddingRepository, EmbeddingRepository>();
        services.AddScoped<IVectorIndexRepository, VectorIndexRepository>();
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<ITokenUsageRepository, TokenUsageRepository>();
        services.AddScoped<IAIAuditRepository, AIAuditRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
