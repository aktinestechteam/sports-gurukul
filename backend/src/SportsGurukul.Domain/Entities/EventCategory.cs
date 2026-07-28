using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EventCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EventCategoryType CategoryType { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
