using System.Text.Json;
using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class CoachAvailability : BaseEntity
{
    public Guid CoachId { get; set; }
    public string WeeklySchedule { get; set; } = string.Empty;
    public string TimeSlots { get; set; } = string.Empty;
    public bool OnlineAvailable { get; set; } = true;
    public bool OfflineAvailable { get; set; } = true;
    public int? TravelDistance { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Coach Coach { get; set; } = null!;
}
