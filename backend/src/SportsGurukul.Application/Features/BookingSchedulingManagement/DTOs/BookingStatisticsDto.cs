namespace SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

public class BookingStatisticsDto
{
    public int TotalBookings { get; set; }
    public int DailyBookings { get; set; }
    public int MonthlyBookings { get; set; }
    public decimal FacilityUtilizationPercent { get; set; }
    public decimal CoachUtilizationPercent { get; set; }
    public decimal CancellationRate { get; set; }
    public decimal WaitlistConversionRate { get; set; }
    public IReadOnlyList<PeakHourDto> PeakBookingHours { get; set; } = [];
    public IReadOnlyList<FacilityOccupancyDto> FacilityOccupancy { get; set; } = [];
}

public class PeakHourDto
{
    public int Hour { get; set; }
    public int BookingCount { get; set; }
}

public class FacilityOccupancyDto
{
    public Guid FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public decimal OccupancyPercent { get; set; }
}
