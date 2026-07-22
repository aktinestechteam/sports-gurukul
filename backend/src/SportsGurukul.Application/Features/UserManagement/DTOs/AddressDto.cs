using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.DTOs;

public class AddressDto
{
    public Guid Id { get; set; }
    public AddressType AddressType { get; set; }
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool IsPrimary { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
