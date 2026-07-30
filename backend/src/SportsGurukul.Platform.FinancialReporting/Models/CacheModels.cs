namespace SportsGurukul.Platform.FinancialReporting.Models;

public class CacheOptions
{
    public TimeSpan AbsoluteExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(2);
    public string? Region { get; set; }
}

public enum CacheRegion
{
    Dashboard, RevenueSummary, MonthlyReports, TaxSummary, Analytics
}
