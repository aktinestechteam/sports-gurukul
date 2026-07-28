using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventTicket : BaseEntity
{
    public Guid EventId { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public string TicketType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int QuantityAvailable { get; set; }
    public int QuantitySold { get; set; }
    public int? MaxPerPerson { get; set; }
    public DateTime? SaleStartDate { get; set; }
    public DateTime? SaleEndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
}
