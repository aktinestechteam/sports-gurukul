using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Behaviors;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Ics;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

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

        return services;
    }
}
