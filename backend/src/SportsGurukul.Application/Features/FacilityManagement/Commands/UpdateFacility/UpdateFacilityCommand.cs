using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateFacility;

public class UpdateFacilityCommand : IRequest<Result<FacilityDetailDto>>
{
    public Guid FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public FacilityType? FacilityType { get; set; }
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public IndoorOutdoor? IndoorOutdoor { get; set; }
    public string? SurfaceType { get; set; }
    public bool? LightingAvailable { get; set; }
    public bool? ParkingAvailable { get; set; }
    public bool? ChangingRoomAvailable { get; set; }
    public bool? WashroomAvailable { get; set; }
    public bool? MedicalRoomAvailable { get; set; }
    public FacilityStatus? Status { get; set; }
}
