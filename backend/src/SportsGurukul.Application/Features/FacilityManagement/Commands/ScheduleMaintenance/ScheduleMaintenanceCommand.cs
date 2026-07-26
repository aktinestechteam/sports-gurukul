using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.ScheduleMaintenance;

public class ScheduleMaintenanceCommand : IRequest<Result<MaintenanceDto>>
{
    public Guid EquipmentId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Cost { get; set; }
    public string? PerformedBy { get; set; }
    public string? Notes { get; set; }
}
