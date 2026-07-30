namespace SportsGurukul.Platform.Communication.Analytics.Configuration;

public class SchedulingOptions
{
    public string DefaultTimeZone { get; set; } = "UTC";
    public int DefaultMaxRetries { get; set; } = 3;
    public int DefaultRetryDelayMinutes { get; set; } = 5;
    public int MaxRecurringExecutions { get; set; } = 1000;
    public TimeSpan BusinessHoursStart { get; set; } = new(9, 0, 0);
    public TimeSpan BusinessHoursEnd { get; set; } = new(17, 0, 0);
    public List<DayOfWeek> BusinessDays { get; set; } = new()
    {
        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
        DayOfWeek.Thursday, DayOfWeek.Friday
    };
    public TimeSpan QuietHoursStart { get; set; } = new(22, 0, 0);
    public TimeSpan QuietHoursEnd { get; set; } = new(7, 0, 0);
    public int CheckIntervalSeconds { get; set; } = 60;
}
