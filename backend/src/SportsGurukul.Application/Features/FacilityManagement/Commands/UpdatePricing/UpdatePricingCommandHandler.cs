using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdatePricing;

public class UpdatePricingCommandHandler : IRequestHandler<UpdatePricingCommand, Result<PricingDto>>
{
    private readonly IFacilityPricingRepository _pricingRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePricingCommandHandler> _logger;

    public UpdatePricingCommandHandler(
        IFacilityPricingRepository pricingRepository,
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePricingCommandHandler> logger)
    {
        _pricingRepository = pricingRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PricingDto>> Handle(UpdatePricingCommand request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<PricingDto>.Failure("Facility not found.");
        }

        var pricing = new FacilityPricing
        {
            Id = Guid.NewGuid(),
            FacilityId = request.FacilityId,
            PricingName = request.PricingName,
            HourlyRate = request.HourlyRate,
            DailyRate = request.DailyRate,
            MonthlyRate = request.MonthlyRate,
            PeakHourlyRate = request.PeakHourlyRate,
            OffPeakHourlyRate = request.OffPeakHourlyRate,
            Description = request.Description,
            IsActive = true
        };

        await _pricingRepository.AddAsync(pricing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pricing added with Id: {PricingId} for Facility: {FacilityId}", pricing.Id, request.FacilityId);

        var dto = new PricingDto
        {
            Id = pricing.Id,
            FacilityId = pricing.FacilityId,
            PricingName = pricing.PricingName,
            HourlyRate = pricing.HourlyRate,
            DailyRate = pricing.DailyRate,
            MonthlyRate = pricing.MonthlyRate,
            PeakHourlyRate = pricing.PeakHourlyRate,
            OffPeakHourlyRate = pricing.OffPeakHourlyRate,
            Description = pricing.Description,
            IsActive = pricing.IsActive
        };

        return Result<PricingDto>.Success(dto);
    }
}
