using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilityEquipment;

public class GetFacilityEquipmentQuery : IRequest<Result<IReadOnlyList<EquipmentDto>>>
{
    public Guid FacilityId { get; set; }
}
