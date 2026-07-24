using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class CoachLocation : BaseEntity
{
    public Guid CoachId { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public Coach Coach { get; set; } = null!;
}
