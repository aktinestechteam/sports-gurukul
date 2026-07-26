using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateFacility;

public class UpdateFacilityCommand : IRequest<Result<FacilityDto>>
{
    public Guid FacilityId { get; set; }
    public Guid AcademyId { get; set; }
    public string? FacilityName { get; set; }
    public AcademyFacilityType? FacilityType { get; set; }
    public string? IndoorOutdoor { get; set; }
    public int? Capacity { get; set; }
    public bool? Available { get; set; }
    public string? Description { get; set; }
}
