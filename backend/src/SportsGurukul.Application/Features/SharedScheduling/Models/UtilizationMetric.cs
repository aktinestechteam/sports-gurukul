namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record UtilizationMetric
{
    public Guid ResourceId { get; init; }
    public string ResourceType { get; init; } = string.Empty;
    public string? ResourceName { get; init; }
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public int TotalSlots { get; init; }
    public int BookedSlots { get; init; }
    public int AvailableSlots => TotalSlots - BookedSlots;
    public decimal UtilizationPercent => TotalSlots > 0 ? Math.Round((decimal)BookedSlots / TotalSlots * 100, 2) : 0;
    public IReadOnlyList<PeakHourInfo> PeakHours { get; init; } = [];
}

public sealed record PeakHourInfo
{
    public int Hour { get; init; }
    public int BookingCount { get; init; }
    public bool IsPeak => BookingCount > 5;
}
