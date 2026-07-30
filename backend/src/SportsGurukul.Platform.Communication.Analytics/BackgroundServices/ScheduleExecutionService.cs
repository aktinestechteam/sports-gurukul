using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.BackgroundServices;

public class ScheduleExecutionOptions
{
    public int CheckIntervalSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
}

public sealed class ScheduleExecutionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduleExecutionService> _logger;
    private readonly ScheduleExecutionOptions _options;

    private long _totalRuns;
    private long _successfulRuns;
    private long _failedRuns;

    public ScheduleExecutionService(
        IServiceScopeFactory scopeFactory,
        ILogger<ScheduleExecutionService> logger,
        IOptions<ScheduleExecutionOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScheduleExecutionService started. Checking every {Interval}s.", _options.CheckIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueJobsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing due jobs.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.CheckIntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessDueJobsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var engine = scope.ServiceProvider.GetRequiredService<ISchedulingEngine>();

        List<ScheduleJobDto> dueJobs;
        try
        {
            dueJobs = await engine.GetDueJobsAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve due jobs.");
            return;
        }

        foreach (var job in dueJobs)
        {
            await ProcessJobAsync(engine, job, ct);
        }
    }

    private async Task ProcessJobAsync(ISchedulingEngine engine, ScheduleJobDto job, CancellationToken ct)
    {
        Interlocked.Increment(ref _totalRuns);

        try
        {
            var validation = await engine.ValidateScheduleAsync(job.Schedule, ct);

            if (!validation.IsValid)
            {
                _logger.LogWarning(
                    "Job {JobId} ({JobType}) skipped: {Errors}",
                    job.Id, job.JobType, string.Join("; ", validation.Errors));
                return;
            }

            if (validation.ConflictsWithQuietHours)
            {
                _logger.LogInformation("Job {JobId} skipped: conflicts with quiet hours.", job.Id);
                return;
            }

            if (validation.ConflictsWithHoliday)
            {
                _logger.LogInformation("Job {JobId} skipped: conflicts with holiday.", job.Id);
                return;
            }

            var result = await AttemptExecutionWithRetryAsync(engine, job, ct);

            if (result.Success)
            {
                Interlocked.Increment(ref _successfulRuns);
                _logger.LogInformation(
                    "Job {JobId} executed successfully in {DurationMs}ms.",
                    job.Id, result.DurationMs);
            }
            else
            {
                Interlocked.Increment(ref _failedRuns);
                _logger.LogError(
                    "Job {JobId} failed after {AttemptNumber} attempt(s): {Error}",
                    job.Id, result.AttemptNumber, result.Error);
            }
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _failedRuns);
            _logger.LogError(ex, "Unexpected error processing job {JobId}.", job.Id);
        }
    }

    private async Task<ScheduleExecutionResult> AttemptExecutionWithRetryAsync(
        ISchedulingEngine engine, ScheduleJobDto job, CancellationToken ct)
    {
        ScheduleExecutionResult? lastResult = null;

        for (var attempt = 1; attempt <= _options.MaxRetries; attempt++)
        {
            try
            {
                lastResult = await engine.ExecuteScheduledAsync(job.Id, DateTime.UtcNow, ct);

                if (lastResult.Success)
                    return lastResult;
            }
            catch (Exception ex) when (attempt < _options.MaxRetries)
            {
                _logger.LogWarning(ex,
                    "Attempt {Attempt}/{MaxRetries} failed for job {JobId}. Retrying.",
                    attempt, _options.MaxRetries, job.Id);
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), ct);
        }

        return lastResult ?? new ScheduleExecutionResult(
            job.Id, false, DateTime.UtcNow, null, 0,
            "All retry attempts exhausted.", _options.MaxRetries);
    }
}
