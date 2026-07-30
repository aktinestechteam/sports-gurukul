using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Behaviors;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Ics;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Application.Features.FinanceManagement.Services;
using SportsGurukul.Application.Features.TournamentManagement.Services;

namespace SportsGurukul.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient<IAvailabilityService, AvailabilityService>();
        services.AddTransient<IBookingApprovalService, BookingApprovalService>();
        services.AddTransient<IConflictDetectionService, ConflictDetectionService>();
        services.AddTransient<IRecurrenceService, RecurrenceService>();
        services.AddTransient<ISchedulingEngine, SchedulingEngine>();
        services.AddTransient<IWaitlistService, WaitlistService>();

        services.AddTransient<Features.SharedScheduling.Engine.IAvailabilityEngine, Features.SharedScheduling.Engine.AvailabilityEngine>();
        services.AddTransient<Features.SharedScheduling.Engine.IBusinessHoursProvider, Features.SharedScheduling.Engine.BusinessHoursProvider>();
        services.AddTransient<Features.SharedScheduling.Engine.ICalendarEngine, Features.SharedScheduling.Engine.CalendarEngine>();
        services.AddTransient<Features.SharedScheduling.Engine.IConflictDetectionEngine, Features.SharedScheduling.Engine.ConflictDetectionEngine>();
        services.AddTransient<Features.SharedScheduling.Engine.IHolidayProvider, Features.SharedScheduling.Engine.DefaultHolidayProvider>();
        services.AddTransient<Features.SharedScheduling.Engine.IOptimizationEngine, Features.SharedScheduling.Engine.OptimizationEngine>();
        services.AddTransient<Features.SharedScheduling.Engine.IRecurrenceEngine, Features.SharedScheduling.Engine.RecurrenceEngine>();
        services.AddTransient<Features.SharedScheduling.Engine.ITimeSlotGenerator, Features.SharedScheduling.Engine.TimeSlotGenerator>();
        services.AddTransient<Features.SharedScheduling.Engine.ITimeZoneService, Features.SharedScheduling.Engine.TimeZoneService>();
        services.AddTransient<Features.SharedScheduling.Engine.ISchedulingEngine, Features.SharedScheduling.Engine.SchedulingEngine>();

        services.AddTransient<ICalendarExporter, IcsExporter>();
        services.AddTransient<ICalendarImporter, IcsImporter>();

        services.AddTransient<IBracketGenerationService, StubBracketGenerationService>();
        services.AddTransient<IFixtureGenerationService, StubFixtureGenerationService>();
        services.AddTransient<ISeedingService, StubSeedingService>();
        services.AddTransient<IRankingCalculationService, StubRankingCalculationService>();
        services.AddTransient<IScoringService, StubScoringService>();

        services.AddTransient<IEventLifecycleService, EventLifecycleService>();
        services.AddTransient<IEventRegistrationService, EventRegistrationService>();
        services.AddTransient<IEventAttendanceService, EventAttendanceService>();
        services.AddTransient<IEventCertificateService, EventCertificateService>();
        services.AddTransient<IEventFeedbackService, EventFeedbackService>();
        services.AddTransient<IEventAnnouncementService, EventAnnouncementService>();

        services.AddTransient<Features.RegistrationAttendancePlatform.Engines.IRegistrationEngine, Features.RegistrationAttendancePlatform.Engines.RegistrationEngine>();
        services.AddTransient<Features.RegistrationAttendancePlatform.Engines.IAttendanceEngine, Features.RegistrationAttendancePlatform.Engines.AttendanceEngine>();
        services.AddTransient<Features.RegistrationAttendancePlatform.Engines.ICheckInService, Features.RegistrationAttendancePlatform.Engines.CheckInService>();
        services.AddTransient<Features.RegistrationAttendancePlatform.Engines.ICheckOutService, Features.RegistrationAttendancePlatform.Engines.CheckOutService>();
        services.AddTransient<Features.RegistrationAttendancePlatform.Engines.ICertificateEngine, Features.RegistrationAttendancePlatform.Engines.CertificateEngine>();
        services.AddTransient<Features.RegistrationAttendancePlatform.Engines.IQrCodeService, Features.RegistrationAttendancePlatform.Engines.QrCodeService>();
        services.AddTransient<Features.RegistrationAttendancePlatform.Engines.ICapacityManagementService, Features.RegistrationAttendancePlatform.Engines.CapacityManagementService>();
        services.AddTransient<Features.RegistrationAttendancePlatform.Engines.IWaitlistEngine, Features.RegistrationAttendancePlatform.Engines.WaitlistEngine>();

        services.AddTransient<Features.EventSearchDiscovery.Engines.IRecommendationEngine, Features.EventSearchDiscovery.Engines.RecommendationEngine>();
        services.AddTransient<Features.EventSearchDiscovery.Engines.IPersonalizationService, Features.EventSearchDiscovery.Engines.PersonalizationService>();
        services.AddTransient<Features.EventSearchDiscovery.Engines.IRecommendationStrategy, Features.EventSearchDiscovery.Engines.EventScoringEngine>();
        services.AddTransient<Features.EventSearchDiscovery.Engines.IRecommendationStrategy, Features.EventSearchDiscovery.Engines.PopularityScoringEngine>();

        services.AddTransient<IInvoiceService, InvoiceService>();
        services.AddTransient<IPaymentService, PaymentService>();
        services.AddTransient<IRefundService, RefundService>();
        services.AddTransient<IWalletService, WalletService>();
        services.AddTransient<ICouponService, CouponService>();
        services.AddTransient<ILedgerService, LedgerService>();
        services.AddTransient<ITaxCalculationService, TaxCalculationService>();
        services.AddTransient<IDiscountService, DiscountService>();
        services.AddTransient<ISettlementService, SettlementService>();

        return services;
    }
}
