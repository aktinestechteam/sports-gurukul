using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateFacility;

public class CreateFacilityCommand : IRequest<Result<FacilityDto>>
{
    public Guid AcademyId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public AcademyFacilityType FacilityType { get; set; }
    public string? IndoorOutdoor { get; set; }
    public int? Capacity { get; set; }
    public bool Available { get; set; } = true;
    public string? Description { get; set; }
}
