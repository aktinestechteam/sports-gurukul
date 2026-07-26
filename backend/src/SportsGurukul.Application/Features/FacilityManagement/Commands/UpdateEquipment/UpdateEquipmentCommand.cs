using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateEquipment;

public class UpdateEquipmentCommand : IRequest<Result<EquipmentDto>>
{
    public Guid EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public string? Category { get; set; }
    public EquipmentCondition? Condition { get; set; }
    public string? MaintenanceSchedule { get; set; }
    public DateTime? WarrantyExpiry { get; set; }
    public int? Quantity { get; set; }
    public EquipmentStatus? Status { get; set; }
    public string? Description { get; set; }
}
