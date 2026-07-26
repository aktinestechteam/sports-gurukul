using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilityPricing;

public class GetFacilityPricingQuery : IRequest<Result<IReadOnlyList<PricingDto>>>
{
    public Guid FacilityId { get; set; }
}
