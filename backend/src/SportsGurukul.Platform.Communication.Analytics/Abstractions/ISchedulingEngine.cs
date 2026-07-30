using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface ISchedulingEngine
{
    Task<ScheduleValidationResult> ValidateScheduleAsync(ScheduleDefinitionDto schedule, CancellationToken ct = default);
    Task<ScheduleExecutionResult> ExecuteImmediateAsync(Guid jobId, CancellationToken ct = default);
    Task<ScheduleExecutionResult> ExecuteScheduledAsync(Guid jobId, DateTime scheduledTime, CancellationToken ct = default);
    Task<ScheduleExecutionResult> ExecuteRecurringAsync(Guid jobId, CancellationToken ct = default);
    Task<List<DateTime>> CalculateNextOccurrencesAsync(ScheduleDefinitionDto schedule, int count = 5, CancellationToken ct = default);
    Task<DateTime?> CalculateNextRunAsync(ScheduleDefinitionDto schedule, CancellationToken ct = default);
    Task<BusinessHoursDto> GetBusinessHoursAsync(CancellationToken ct = default);
    Task<BusinessHoursDto> SetBusinessHoursAsync(BusinessHoursDto hours, CancellationToken ct = default);
    Task<bool> IsWithinBusinessHoursAsync(DateTime dateTime, string? timeZone, CancellationToken ct = default);
    Task<QuietHoursDto> GetQuietHoursAsync(CancellationToken ct = default);
    Task<QuietHoursDto> SetQuietHoursAsync(QuietHoursDto quietHours, CancellationToken ct = default);
    Task<bool> IsQuietHoursAsync(DateTime dateTime, string? timeZone, CancellationToken ct = default);
    Task<HolidayCalendarDto> GetHolidayCalendarAsync(int year, string? country, CancellationToken ct = default);
    Task<HolidayCalendarDto> SetHolidayCalendarAsync(HolidayCalendarDto calendar, CancellationToken ct = default);
    Task<bool> IsHolidayAsync(DateTime date, string? country, CancellationToken ct = default);
    Task<TimeZoneInfoDto> GetTimeZoneInfoAsync(string timeZoneId, CancellationToken ct = default);
    Task<List<TimeZoneInfoDto>> GetAvailableTimeZonesAsync(CancellationToken ct = default);
    Task<DateTime> ConvertToTimeZoneAsync(DateTime utcDateTime, string targetTimeZone, CancellationToken ct = default);
    Task<DateTime> ConvertFromTimeZoneAsync(DateTime localDateTime, string sourceTimeZone, CancellationToken ct = default);
    Task<RetryWindowDto> GetRetryPolicyAsync(CancellationToken ct = default);
    Task<RetryWindowDto> SetRetryPolicyAsync(RetryWindowDto policy, CancellationToken ct = default);
    Task<List<ScheduleJobDto>> GetDueJobsAsync(CancellationToken ct = default);
    Task<ScheduleJobDto> RegisterJobAsync(Guid campaignId, ScheduleDefinitionDto schedule, CancellationToken ct = default);
    Task<bool> UnregisterJobAsync(Guid jobId, CancellationToken ct = default);
}
