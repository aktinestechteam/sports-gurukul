using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class FacilityPricing : BaseEntity
{
    public Guid FacilityId { get; set; }
    public string PricingName { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public decimal? PeakHourlyRate { get; set; }
    public decimal? OffPeakHourlyRate { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Facility Facility { get; set; } = null!;
}
