using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateFacilitySchedule;

public class UpdateFacilityScheduleCommand : IRequest<Result<ScheduleDto>>
{
    public Guid FacilityId { get; set; }
    public int DayOfWeek { get; set; }
    public string OpeningTime { get; set; } = string.Empty;
    public string ClosingTime { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
    public bool IsMaintenanceWindow { get; set; }
    public string? Notes { get; set; }
}
