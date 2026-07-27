using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Behaviors;
using SportsGurukul.Application.Common.Interfaces;
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

        return services;
    }
}
