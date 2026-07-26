using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilityPricing;

public class GetFacilityPricingQueryHandler : IRequestHandler<GetFacilityPricingQuery, Result<IReadOnlyList<PricingDto>>>
{
    private readonly IFacilityPricingRepository _pricingRepository;
    private readonly ILogger<GetFacilityPricingQueryHandler> _logger;

    public GetFacilityPricingQueryHandler(
        IFacilityPricingRepository pricingRepository,
        ILogger<GetFacilityPricingQueryHandler> logger)
    {
        _pricingRepository = pricingRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<PricingDto>>> Handle(GetFacilityPricingQuery request, CancellationToken cancellationToken)
    {
        var pricingTiers = await _pricingRepository.GetByFacilityIdAsync(request.FacilityId, cancellationToken);

        var dtos = pricingTiers.Select(p => new PricingDto
        {
            Id = p.Id,
            FacilityId = p.FacilityId,
            PricingName = p.PricingName,
            HourlyRate = p.HourlyRate,
            DailyRate = p.DailyRate,
            MonthlyRate = p.MonthlyRate,
            PeakHourlyRate = p.PeakHourlyRate,
            OffPeakHourlyRate = p.OffPeakHourlyRate,
            Description = p.Description,
            IsActive = p.IsActive
        }).ToList();

        _logger.LogInformation("Retrieved {Count} pricing tiers for Facility: {FacilityId}", dtos.Count, request.FacilityId);

        return Result<IReadOnlyList<PricingDto>>.Success(dtos);
    }
}
