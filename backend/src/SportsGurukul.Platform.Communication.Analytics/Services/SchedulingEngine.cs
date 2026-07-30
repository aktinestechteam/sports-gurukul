using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.Configuration;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class SchedulingEngine : ISchedulingEngine
{
    private readonly ILogger<SchedulingEngine> _logger;
    private readonly ICacheService _cache;
    private readonly SchedulingOptions _options;
    private readonly ConcurrentDictionary<Guid, ScheduleJobDto> _jobs = new();
    private readonly ConcurrentDictionary<string, HolidayCalendarDto> _holidayCalendars = new();

    private BusinessHoursDto _businessHours;
    private QuietHoursDto _quietHours;
    private RetryWindowDto _retryPolicy;
    private readonly Lock _businessHoursLock = new();
    private readonly Lock _quietHoursLock = new();
    private readonly Lock _retryPolicyLock = new();

    public SchedulingEngine(ILogger<SchedulingEngine> logger, ICacheService cache, IOptions<SchedulingOptions> options)
    {
        _logger = logger;
        _cache = cache;
        _options = options.Value;

        var weekHours = new Dictionary<DayOfWeek, TimeRangeDto>();
        foreach (var day in _options.BusinessDays)
        {
            weekHours[day] = new TimeRangeDto(_options.BusinessHoursStart, _options.BusinessHoursEnd, true);
        }

        _businessHours = new BusinessHoursDto(
            Guid.NewGuid(),
            "Default Business Hours",
            weekHours,
            null,
            _options.DefaultTimeZone,
            true
        );

        _quietHours = new QuietHoursDto(
            Guid.NewGuid(),
            "Default Quiet Hours",
            _options.QuietHoursStart,
            _options.QuietHoursEnd,
            _options.DefaultTimeZone,
            _options.BusinessDays,
            true,
            false
        );

        _retryPolicy = new RetryWindowDto(
            Guid.NewGuid(),
            "Default Retry Policy",
            _options.DefaultMaxRetries,
            TimeSpan.FromMinutes(_options.DefaultRetryDelayMinutes),
            TimeSpan.FromMinutes(30),
            2.0,
            RetryBackoffStrategy.Exponential,
            null,
            true
        );
    }

    public Task<ScheduleValidationResult> ValidateScheduleAsync(ScheduleDefinitionDto schedule, CancellationToken ct = default)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        DateTime? nextOccurrence = null;

        if (!string.IsNullOrEmpty(schedule.CronExpression))
        {
            if (!TryParseCronFields(schedule.CronExpression, out var _, out var cronError))
            {
                errors.Add(cronError ?? "Invalid cron expression");
            }
        }

        if (!string.IsNullOrEmpty(schedule.TimeZone))
        {
            try
            {
                _ = TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZone);
            }
            catch
            {
                errors.Add($"Invalid timezone: {schedule.TimeZone}");
            }
        }

        if (schedule.IntervalMinutes.HasValue && schedule.IntervalMinutes.Value <= 0)
        {
            errors.Add("IntervalMinutes must be greater than 0");
        }

        if (schedule.StartDate.HasValue && schedule.EndDate.HasValue && schedule.StartDate >= schedule.EndDate)
        {
            errors.Add("StartDate must be before EndDate");
        }

        if (schedule.Pattern.HasValue && schedule.Pattern == RecurrencePattern.Weekly
            && (schedule.DaysOfWeek == null || schedule.DaysOfWeek.Count == 0))
        {
            errors.Add("DaysOfWeek must be specified for weekly recurrence");
        }

        if (string.IsNullOrEmpty(schedule.CronExpression) && !schedule.IntervalMinutes.HasValue && !schedule.Pattern.HasValue)
        {
            errors.Add("Schedule must specify either CronExpression, IntervalMinutes, or Pattern");
        }

        if (errors.Count == 0)
        {
            try
            {
                nextOccurrence = CalculateNextCronOrInterval(schedule, DateTime.UtcNow);
            }
            catch
            {
                // Next occurrence calculation failed but validation passed
            }
        }

        var conflictsQuietHours = false;
        var conflictsHoliday = false;

        if (nextOccurrence.HasValue)
        {
            conflictsQuietHours = IsTimeInQuietHours(nextOccurrence.Value, schedule.TimeZone);
            conflictsHoliday = IsDateHoliday(nextOccurrence.Value.Date);
        }

        var result = new ScheduleValidationResult(
            errors.Count == 0,
            warnings,
            errors,
            nextOccurrence,
            null,
            0,
            conflictsQuietHours,
            conflictsHoliday
        );

        return Task.FromResult(result);
    }

    public async Task<ScheduleExecutionResult> ExecuteImmediateAsync(Guid jobId, CancellationToken ct = default)
    {
        if (!_jobs.TryGetValue(jobId, out var job))
        {
            return new ScheduleExecutionResult(jobId, false, DateTime.UtcNow, null, 0, "Job not found", 0);
        }

        var sw = Stopwatch.StartNew();
        try
        {
            await SimulateExecutionAsync(ct);
            sw.Stop();

            var updated = job with
            {
                LastRunAt = DateTime.UtcNow,
                TotalRuns = job.TotalRuns + 1,
                SuccessfulRuns = job.SuccessfulRuns + 1
            };
            _jobs.TryUpdate(jobId, updated, job);

            return new ScheduleExecutionResult(jobId, true, DateTime.UtcNow, updated.NextRunAt, sw.ElapsedMilliseconds, null, 1);
        }
        catch (Exception ex)
        {
            sw.Stop();
            var updated = job with
            {
                FailedRuns = job.FailedRuns + 1
            };
            _jobs.TryUpdate(jobId, updated, job);

            return new ScheduleExecutionResult(jobId, false, DateTime.UtcNow, job.NextRunAt, sw.ElapsedMilliseconds, ex.Message, 1);
        }
    }

    public async Task<ScheduleExecutionResult> ExecuteScheduledAsync(Guid jobId, DateTime scheduledTime, CancellationToken ct = default)
    {
        if (!_jobs.TryGetValue(jobId, out var job))
        {
            return new ScheduleExecutionResult(jobId, false, DateTime.UtcNow, null, 0, "Job not found", 0);
        }

        var sw = Stopwatch.StartNew();
        try
        {
            await SimulateExecutionAsync(ct);
            sw.Stop();

            var nextRun = await CalculateNextRunAsync(job.Schedule, ct);
            var updated = job with
            {
                LastRunAt = DateTime.UtcNow,
                NextRunAt = nextRun,
                TotalRuns = job.TotalRuns + 1,
                SuccessfulRuns = job.SuccessfulRuns + 1
            };
            _jobs.TryUpdate(jobId, updated, job);

            return new ScheduleExecutionResult(jobId, true, DateTime.UtcNow, nextRun, sw.ElapsedMilliseconds, null, 1);
        }
        catch (Exception ex)
        {
            sw.Stop();
            var updated = job with
            {
                FailedRuns = job.FailedRuns + 1
            };
            _jobs.TryUpdate(jobId, updated, job);

            return new ScheduleExecutionResult(jobId, false, DateTime.UtcNow, job.NextRunAt, sw.ElapsedMilliseconds, ex.Message, 1);
        }
    }

    public async Task<ScheduleExecutionResult> ExecuteRecurringAsync(Guid jobId, CancellationToken ct = default)
    {
        if (!_jobs.TryGetValue(jobId, out var job))
        {
            return new ScheduleExecutionResult(jobId, false, DateTime.UtcNow, null, 0, "Job not found", 0);
        }

        var maxExecutions = _options.MaxRecurringExecutions;
        if (job.TotalRuns >= maxExecutions)
        {
            var deactivated = job with { IsActive = false };
            _jobs.TryUpdate(jobId, deactivated, job);

            return new ScheduleExecutionResult(jobId, false, DateTime.UtcNow, null, 0, $"Max recurring executions ({maxExecutions}) reached", 0);
        }

        var sw = Stopwatch.StartNew();
        try
        {
            await SimulateExecutionAsync(ct);
            sw.Stop();

            var nextRun = await CalculateNextRunAsync(job.Schedule, ct);
            var updated = job with
            {
                LastRunAt = DateTime.UtcNow,
                NextRunAt = nextRun,
                TotalRuns = job.TotalRuns + 1,
                SuccessfulRuns = job.SuccessfulRuns + 1
            };

            if (!nextRun.HasValue)
            {
                updated = updated with { IsActive = false };
            }

            _jobs.TryUpdate(jobId, updated, job);

            return new ScheduleExecutionResult(jobId, true, DateTime.UtcNow, nextRun, sw.ElapsedMilliseconds, null, job.TotalRuns + 1);
        }
        catch (Exception ex)
        {
            sw.Stop();
            var retryDelay = CalculateRetryDelay(job.FailedRuns);
            var nextRetry = DateTime.UtcNow.Add(retryDelay);
            var updated = job with
            {
                NextRunAt = nextRetry,
                FailedRuns = job.FailedRuns + 1
            };
            _jobs.TryUpdate(jobId, updated, job);

            return new ScheduleExecutionResult(jobId, false, DateTime.UtcNow, nextRetry, sw.ElapsedMilliseconds, ex.Message, job.FailedRuns + 1);
        }
    }

    public async Task<List<DateTime>> CalculateNextOccurrencesAsync(ScheduleDefinitionDto schedule, int count = 5, CancellationToken ct = default)
    {
        var occurrences = new List<DateTime>();
        var from = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(schedule.CronExpression))
        {
            if (!TryParseCronFields(schedule.CronExpression, out var fields, out _))
                return occurrences;

            var current = from;
            while (occurrences.Count < count)
            {
                var next = FindNextCronMatch(fields.Value, current);
                if (!next.HasValue)
                    break;

                if (schedule.EndDate.HasValue && next.Value > schedule.EndDate.Value)
                    break;

                occurrences.Add(next.Value);
                current = next.Value.AddMinutes(1);
            }
        }
        else if (schedule.IntervalMinutes.HasValue && schedule.StartDate.HasValue)
        {
            var start = schedule.StartDate.Value > from ? schedule.StartDate.Value : from;
            var current = start;

            while (occurrences.Count < count)
            {
                if (schedule.EndDate.HasValue && current > schedule.EndDate.Value)
                    break;

                if (current > from)
                    occurrences.Add(current);

                current = current.AddMinutes(schedule.IntervalMinutes.Value);
            }
        }
        else if (schedule.Pattern.HasValue)
        {
            occurrences = CalculatePatternOccurrences(schedule, from, count);
        }

        return occurrences;
    }

    public Task<DateTime?> CalculateNextRunAsync(ScheduleDefinitionDto schedule, CancellationToken ct = default)
    {
        var next = CalculateNextCronOrInterval(schedule, DateTime.UtcNow);
        return Task.FromResult(next);
    }

    public Task<BusinessHoursDto> GetBusinessHoursAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_businessHours);
    }

    public Task<BusinessHoursDto> SetBusinessHoursAsync(BusinessHoursDto hours, CancellationToken ct = default)
    {
        lock (_businessHoursLock)
        {
            _businessHours = hours;
        }
        return Task.FromResult(hours);
    }

    public Task<bool> IsWithinBusinessHoursAsync(DateTime dateTime, string? timeZone, CancellationToken ct = default)
    {
        var dt = dateTime;
        if (!string.IsNullOrEmpty(timeZone))
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                dt = TimeZoneInfo.ConvertTimeFromUtc(dt, tz);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        if (!_businessHours.WeekHours.TryGetValue(dt.DayOfWeek, out var range) || !range.IsActive)
            return Task.FromResult(false);

        return Task.FromResult(dt.TimeOfDay >= range.Start && dt.TimeOfDay < range.End);
    }

    public Task<QuietHoursDto> GetQuietHoursAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_quietHours);
    }

    public Task<QuietHoursDto> SetQuietHoursAsync(QuietHoursDto quietHours, CancellationToken ct = default)
    {
        lock (_quietHoursLock)
        {
            _quietHours = quietHours;
        }
        return Task.FromResult(quietHours);
    }

    public Task<bool> IsQuietHoursAsync(DateTime dateTime, string? timeZone, CancellationToken ct = default)
    {
        var dt = dateTime;
        if (!string.IsNullOrEmpty(timeZone))
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                dt = TimeZoneInfo.ConvertTimeFromUtc(dt, tz);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(IsTimeInQuietHours(dt, null));
    }

    public Task<HolidayCalendarDto> GetHolidayCalendarAsync(int year, string? country, CancellationToken ct = default)
    {
        var key = BuildHolidayKey(year, country);
        if (_holidayCalendars.TryGetValue(key, out var existing))
            return Task.FromResult(existing);

        var calendar = GenerateDefaultHolidayCalendar(year, country);
        _holidayCalendars[key] = calendar;
        return Task.FromResult(calendar);
    }

    public Task<HolidayCalendarDto> SetHolidayCalendarAsync(HolidayCalendarDto calendar, CancellationToken ct = default)
    {
        var key = BuildHolidayKey(calendar.Year, calendar.Country);
        _holidayCalendars[key] = calendar;
        return Task.FromResult(calendar);
    }

    public Task<bool> IsHolidayAsync(DateTime date, string? country, CancellationToken ct = default)
    {
        return Task.FromResult(IsDateHoliday(date.Date, country));
    }

    public Task<TimeZoneInfoDto> GetTimeZoneInfoAsync(string timeZoneId, CancellationToken ct = default)
    {
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var dto = new TimeZoneInfoDto(tz.Id, tz.DisplayName, tz.BaseUtcOffset, tz.SupportsDaylightSavingTime);
            return Task.FromResult(dto);
        }
        catch
        {
            throw new ArgumentException($"Time zone '{timeZoneId}' not found", nameof(timeZoneId));
        }
    }

    public Task<List<TimeZoneInfoDto>> GetAvailableTimeZonesAsync(CancellationToken ct = default)
    {
        var timeZones = TimeZoneInfo.GetSystemTimeZones()
            .Select(tz => new TimeZoneInfoDto(tz.Id, tz.DisplayName, tz.BaseUtcOffset, tz.SupportsDaylightSavingTime))
            .ToList();
        return Task.FromResult(timeZones);
    }

    public Task<DateTime> ConvertToTimeZoneAsync(DateTime utcDateTime, string targetTimeZone, CancellationToken ct = default)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZone);
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
        return Task.FromResult(local);
    }

    public Task<DateTime> ConvertFromTimeZoneAsync(DateTime localDateTime, string sourceTimeZone, CancellationToken ct = default)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZone);
        var utc = TimeZoneInfo.ConvertTimeToUtc(localDateTime, tz);
        return Task.FromResult(utc);
    }

    public Task<RetryWindowDto> GetRetryPolicyAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_retryPolicy);
    }

    public Task<RetryWindowDto> SetRetryPolicyAsync(RetryWindowDto policy, CancellationToken ct = default)
    {
        lock (_retryPolicyLock)
        {
            _retryPolicy = policy;
        }
        return Task.FromResult(policy);
    }

    public Task<List<ScheduleJobDto>> GetDueJobsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var dueJobs = _jobs.Values
            .Where(j => j.IsActive && j.NextRunAt.HasValue && j.NextRunAt.Value <= now)
            .OrderBy(j => j.NextRunAt)
            .ToList();
        return Task.FromResult(dueJobs);
    }

    public async Task<ScheduleJobDto> RegisterJobAsync(Guid campaignId, ScheduleDefinitionDto schedule, CancellationToken ct = default)
    {
        var nextRun = await CalculateNextRunAsync(schedule, ct);
        var job = new ScheduleJobDto(
            Guid.NewGuid(),
            "Campaign",
            campaignId.ToString(),
            schedule,
            true,
            null,
            nextRun,
            0,
            0,
            0,
            DateTime.UtcNow
        );
        _jobs[job.Id] = job;

        _logger.LogInformation("Registered job {JobId} for campaign {CampaignId}, next run at {NextRun}",
            job.Id, campaignId, nextRun);

        return job;
    }

    public Task<bool> UnregisterJobAsync(Guid jobId, CancellationToken ct = default)
    {
        if (_jobs.TryRemove(jobId, out var removed))
        {
            _logger.LogInformation("Unregistered job {JobId}", jobId);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    private static bool TryParseCronFields(string cron, out (HashSet<int> Minutes, HashSet<int> Hours, HashSet<int> DaysOfMonth, HashSet<int> Months, HashSet<int> DaysOfWeek)? fields, out string? error)
    {
        fields = null;
        error = null;

        var parts = cron.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 5)
        {
            error = "Cron expression must have exactly 5 fields";
            return false;
        }

        try
        {
            var minutes = ParseCronField(parts[0], 0, 59);
            var hours = ParseCronField(parts[1], 0, 23);
            var daysOfMonth = ParseCronField(parts[2], 1, 31);
            var months = ParseCronField(parts[3], 1, 12);
            var daysOfWeek = ParseCronField(parts[4], 0, 6);

            fields = (minutes, hours, daysOfMonth, months, daysOfWeek);
            return true;
        }
        catch (FormatException ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private static HashSet<int> ParseCronField(string field, int min, int max)
    {
        var values = new HashSet<int>();

        foreach (var part in field.Split(','))
        {
            var trimmed = part.Trim();
            if (trimmed == "*")
            {
                for (var i = min; i <= max; i++)
                    values.Add(i);
            }
            else if (trimmed.Contains('/'))
            {
                var stepParts = trimmed.Split('/');
                if (stepParts.Length != 2)
                    throw new FormatException($"Invalid step expression: {trimmed}");

                var step = int.Parse(stepParts[1]);
                if (step <= 0)
                    throw new FormatException($"Step must be positive: {step}");

                int start;
                if (stepParts[0] == "*")
                {
                    start = min;
                }
                else if (stepParts[0].Contains('-'))
                {
                    var range = stepParts[0].Split('-');
                    start = int.Parse(range[0]);
                    var end = int.Parse(range[1]);
                    for (var i = start; i <= end; i += step)
                        values.Add(i);
                    continue;
                }
                else
                {
                    start = int.Parse(stepParts[0]);
                }

                for (var i = start; i <= max; i += step)
                    values.Add(i);
            }
            else if (trimmed.Contains('-'))
            {
                var range = trimmed.Split('-');
                if (range.Length != 2)
                    throw new FormatException($"Invalid range expression: {trimmed}");

                var rangeStart = int.Parse(range[0]);
                var rangeEnd = int.Parse(range[1]);
                for (var i = rangeStart; i <= rangeEnd; i++)
                    values.Add(i);
            }
            else
            {
                var val = int.Parse(trimmed);
                if (val < min || val > max)
                    throw new FormatException($"Value {val} out of range [{min}-{max}]");
                values.Add(val);
            }
        }

        return values;
    }

    private static DateTime? FindNextCronMatch(
        (HashSet<int> Minutes, HashSet<int> Hours, HashSet<int> DaysOfMonth, HashSet<int> Months, HashSet<int> DaysOfWeek) fields,
        DateTime from)
    {
        var current = new DateTime(from.Year, from.Month, from.Day, from.Hour, from.Minute, 0, from.Kind);

        for (var i = 0; i < 525600; i++)
        {
            current = current.AddMinutes(1);

            if (!fields.Months.Contains(current.Month))
                continue;
            if (!fields.DaysOfMonth.Contains(current.Day))
                continue;
            if (!fields.DaysOfWeek.Contains((int)current.DayOfWeek))
                continue;
            if (!fields.Hours.Contains(current.Hour))
                continue;
            if (!fields.Minutes.Contains(current.Minute))
                continue;

            return current;
        }

        return null;
    }

    private DateTime? CalculateNextCronOrInterval(ScheduleDefinitionDto schedule, DateTime from)
    {
        if (!string.IsNullOrEmpty(schedule.CronExpression))
        {
            if (!TryParseCronFields(schedule.CronExpression, out var fields, out _))
                return null;

            var next = FindNextCronMatch(fields!.Value, from);
            if (next.HasValue && schedule.EndDate.HasValue && next.Value > schedule.EndDate.Value)
                return null;

            return next;
        }

        if (schedule.IntervalMinutes.HasValue && schedule.StartDate.HasValue)
        {
            var interval = TimeSpan.FromMinutes(schedule.IntervalMinutes.Value);

            if (schedule.StartDate.Value > from)
                return schedule.StartDate.Value;

            var elapsed = from - schedule.StartDate.Value;
            var intervalsPassed = (long)(elapsed.TotalMinutes / schedule.IntervalMinutes.Value);
            var next = schedule.StartDate.Value.AddMinutes((intervalsPassed + 1) * schedule.IntervalMinutes.Value);

            if (schedule.EndDate.HasValue && next > schedule.EndDate.Value)
                return null;

            return next;
        }

        if (schedule.Pattern.HasValue)
        {
            var occurrences = CalculatePatternOccurrences(schedule, from, 1);
            return occurrences.Count > 0 ? occurrences[0] : null;
        }

        return null;
    }

    private List<DateTime> CalculatePatternOccurrences(ScheduleDefinitionDto schedule, DateTime from, int count)
    {
        var occurrences = new List<DateTime>();
        if (!schedule.Pattern.HasValue)
            return occurrences;

        var start = schedule.StartDate ?? DateTime.UtcNow;
        var timeOfDay = schedule.TimeOfDay ?? new TimeSpan(9, 0, 0);
        var current = new DateTime(from.Year, from.Month, from.Day, timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds, DateTimeKind.Utc);

        if (current < start)
            current = start;

        var maxIterations = count * 366;
        var iterations = 0;

        while (occurrences.Count < count && iterations < maxIterations)
        {
            iterations++;

            if (current <= from)
            {
                current = schedule.Pattern.Value switch
                {
                    RecurrencePattern.Daily => current.AddDays(1),
                    RecurrencePattern.Weekly => current.AddDays(1),
                    RecurrencePattern.Monthly => current.AddMonths(1),
                    RecurrencePattern.Yearly => current.AddYears(1),
                    _ => current.AddDays(1)
                };
                continue;
            }

            bool matches = schedule.Pattern.Value switch
            {
                RecurrencePattern.Daily => true,
                RecurrencePattern.Weekly => schedule.DaysOfWeek == null || schedule.DaysOfWeek.Contains(current.DayOfWeek),
                RecurrencePattern.Monthly => !schedule.DayOfMonth.HasValue || current.Day == schedule.DayOfMonth.Value,
                RecurrencePattern.Yearly => current.DayOfYear == (schedule.StartDate?.DayOfYear ?? current.DayOfYear),
                RecurrencePattern.Custom => true,
                _ => true
            };

            if (matches)
            {
                var candidate = new DateTime(current.Year, current.Month, current.Day,
                    timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds, DateTimeKind.Utc);

                if (!schedule.EndDate.HasValue || candidate <= schedule.EndDate.Value)
                {
                    occurrences.Add(candidate);
                }
            }

            current = schedule.Pattern.Value switch
            {
                RecurrencePattern.Daily => current.AddDays(1),
                RecurrencePattern.Weekly => current.AddDays(1),
                RecurrencePattern.Monthly => current.AddDays(1),
                RecurrencePattern.Yearly => current.AddDays(1),
                _ => current.AddDays(1)
            };
        }

        return occurrences;
    }

    private bool IsTimeInQuietHours(DateTime dateTime, string? timeZone)
    {
        var dt = dateTime;
        if (!string.IsNullOrEmpty(timeZone))
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                dt = TimeZoneInfo.ConvertTimeFromUtc(dt, tz);
            }
            catch
            {
                return false;
            }
        }

        var timeOfDay = dt.TimeOfDay;
        var start = _quietHours.Start;
        var end = _quietHours.End;

        if (start <= end)
            return timeOfDay >= start && timeOfDay < end;

        return timeOfDay >= start || timeOfDay < end;
    }

    private bool IsDateHoliday(DateTime date, string? country = null)
    {
        var key = BuildHolidayKey(date.Year, country);
        if (!_holidayCalendars.TryGetValue(key, out var calendar))
        {
            calendar = GenerateDefaultHolidayCalendar(date.Year, country);
            _holidayCalendars[key] = calendar;
        }

        return calendar.Holidays.Any(h => h.Date.Date == date.Date);
    }

    private static string BuildHolidayKey(int year, string? country)
    {
        return $"{country ?? "US"}_{year}";
    }

    private static HolidayCalendarDto GenerateDefaultHolidayCalendar(int year, string? country)
    {
        var holidays = new List<HolidayDateDto>
        {
            new(new DateTime(year, 1, 1), "New Year's Day", true, "Public"),
            new(new DateTime(year, 12, 25), "Christmas Day", true, "Public"),
            new(new DateTime(year, 12, 31), "New Year's Eve", true, "Public"),
        };

        if (string.IsNullOrEmpty(country) || string.Equals(country, "US", StringComparison.OrdinalIgnoreCase))
        {
            holidays.Add(new(new DateTime(year, 1, 20), "Martin Luther King Jr. Day", false, "Public"));
            holidays.Add(new(new DateTime(year, 2, 17), "Presidents' Day", false, "Public"));
            holidays.Add(new(new DateTime(year, 5, 26), "Memorial Day", false, "Public"));
            holidays.Add(new(new DateTime(year, 7, 4), "Independence Day", true, "Public"));
            holidays.Add(new(new DateTime(year, 9, 1), "Labor Day", false, "Public"));
            holidays.Add(new(new DateTime(year, 11, 27), "Thanksgiving Day", false, "Public"));
        }
        else if (string.Equals(country, "IN", StringComparison.OrdinalIgnoreCase))
        {
            holidays.Add(new(new DateTime(year, 1, 26), "Republic Day", true, "Public"));
            holidays.Add(new(new DateTime(year, 8, 15), "Independence Day", true, "Public"));
            holidays.Add(new(new DateTime(year, 10, 2), "Gandhi Jayanti", true, "Public"));
        }
        else if (string.Equals(country, "GB", StringComparison.OrdinalIgnoreCase))
        {
            holidays.Add(new(new DateTime(year, 1, 1), "New Year's Day", true, "Public"));
            holidays.Add(new(new DateTime(year, 12, 26), "Boxing Day", true, "Public"));
        }

        return new HolidayCalendarDto(
            Guid.NewGuid(),
            $"Default Holidays {year}",
            "Auto-generated common holidays",
            country,
            year,
            holidays,
            true
        );
    }

    private TimeSpan CalculateRetryDelay(int failedAttempts)
    {
        if (!_retryPolicy.ExponentialBackoff)
        {
            if (_retryPolicy.FixedDelays != null && failedAttempts < _retryPolicy.FixedDelays.Count)
                return _retryPolicy.FixedDelays[failedAttempts];

            return _retryPolicy.InitialDelay;
        }

        var delay = _retryPolicy.InitialDelay.TotalSeconds *
                    Math.Pow(_retryPolicy.BackoffMultiplier, failedAttempts);

        delay = Math.Min(delay, _retryPolicy.MaxDelay.TotalSeconds);

        return TimeSpan.FromSeconds(delay);
    }

    private static async Task SimulateExecutionAsync(CancellationToken ct)
    {
        await Task.Delay(50, ct);
    }
}
