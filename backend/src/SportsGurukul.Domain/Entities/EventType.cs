using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventTypeEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
