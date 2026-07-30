using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Queries;

public record ValidateScheduleQuery(ScheduleDefinitionDto Schedule) : IRequest<ScheduleValidationResult>;

public class ValidateScheduleQueryHandler(ISchedulingEngine engine) : IRequestHandler<ValidateScheduleQuery, ScheduleValidationResult>
{
    public Task<ScheduleValidationResult> Handle(ValidateScheduleQuery query, CancellationToken ct)
        => engine.ValidateScheduleAsync(query.Schedule, ct);
}

public record CalculateNextOccurrencesQuery(ScheduleDefinitionDto Schedule, int Count) : IRequest<List<DateTime>>;

public class CalculateNextOccurrencesQueryHandler(ISchedulingEngine engine) : IRequestHandler<CalculateNextOccurrencesQuery, List<DateTime>>
{
    public Task<List<DateTime>> Handle(CalculateNextOccurrencesQuery query, CancellationToken ct)
        => engine.CalculateNextOccurrencesAsync(query.Schedule, query.Count, ct);
}

public record GetBusinessHoursQuery : IRequest<BusinessHoursDto>;

public class GetBusinessHoursQueryHandler(ISchedulingEngine engine) : IRequestHandler<GetBusinessHoursQuery, BusinessHoursDto>
{
    public Task<BusinessHoursDto> Handle(GetBusinessHoursQuery query, CancellationToken ct)
        => engine.GetBusinessHoursAsync(ct);
}

public record GetQuietHoursQuery : IRequest<QuietHoursDto>;

public class GetQuietHoursQueryHandler(ISchedulingEngine engine) : IRequestHandler<GetQuietHoursQuery, QuietHoursDto>
{
    public Task<QuietHoursDto> Handle(GetQuietHoursQuery query, CancellationToken ct)
        => engine.GetQuietHoursAsync(ct);
}

public record GetHolidayCalendarQuery(int Year, string? Country) : IRequest<HolidayCalendarDto>;

public class GetHolidayCalendarQueryHandler(ISchedulingEngine engine) : IRequestHandler<GetHolidayCalendarQuery, HolidayCalendarDto>
{
    public Task<HolidayCalendarDto> Handle(GetHolidayCalendarQuery query, CancellationToken ct)
        => engine.GetHolidayCalendarAsync(query.Year, query.Country, ct);
}

public record GetTimeZonesQuery : IRequest<List<TimeZoneInfoDto>>;

public class GetTimeZonesQueryHandler(ISchedulingEngine engine) : IRequestHandler<GetTimeZonesQuery, List<TimeZoneInfoDto>>
{
    public Task<List<TimeZoneInfoDto>> Handle(GetTimeZonesQuery query, CancellationToken ct)
        => engine.GetAvailableTimeZonesAsync(ct);
}

public record GetRetryPolicyQuery : IRequest<RetryWindowDto>;

public class GetRetryPolicyQueryHandler(ISchedulingEngine engine) : IRequestHandler<GetRetryPolicyQuery, RetryWindowDto>
{
    public Task<RetryWindowDto> Handle(GetRetryPolicyQuery query, CancellationToken ct)
        => engine.GetRetryPolicyAsync(ct);
}

public record GetDueJobsQuery : IRequest<List<ScheduleJobDto>>;

public class GetDueJobsQueryHandler(ISchedulingEngine engine) : IRequestHandler<GetDueJobsQuery, List<ScheduleJobDto>>
{
    public Task<List<ScheduleJobDto>> Handle(GetDueJobsQuery query, CancellationToken ct)
        => engine.GetDueJobsAsync(ct);
}
