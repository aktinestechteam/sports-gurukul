using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetFacilities;

public class GetFacilitiesQuery : IRequest<Result<IReadOnlyList<FacilityDto>>>
{
    public Guid AcademyId { get; set; }
}
