using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventView : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? ViewedByUserId { get; set; }
    public DateTime ViewedAt { get; set; }
    public string? Source { get; set; }
    public string? DeviceType { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
}
