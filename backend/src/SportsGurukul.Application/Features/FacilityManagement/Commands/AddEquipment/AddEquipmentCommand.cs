using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.AddEquipment;

public class AddEquipmentCommand : IRequest<Result<EquipmentDto>>
{
    public Guid FacilityId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public EquipmentCondition Condition { get; set; }
    public string? MaintenanceSchedule { get; set; }
    public DateTime? WarrantyExpiry { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
}
