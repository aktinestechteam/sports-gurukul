using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilityById;

public class GetFacilityByIdQuery : IRequest<Result<FacilityDetailDto>>
{
    public Guid FacilityId { get; set; }
}
