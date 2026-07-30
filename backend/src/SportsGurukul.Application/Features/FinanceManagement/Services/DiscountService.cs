using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class DiscountService : IDiscountService
{
    private readonly ICouponRepository _couponRepository;

    public DiscountService(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Result<DiscountResult>> ApplyDiscountAsync(decimal subTotal, string couponCode, string? userId, CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByCodeAsync(couponCode, cancellationToken);
        if (coupon is null)
            return Result<DiscountResult>.Failure("Coupon not found");

        if (!coupon.IsActive)
            return Result<DiscountResult>.Failure("Coupon is not active");

        decimal discountAmount = coupon.Type switch
        {
            DiscountType.Percentage => subTotal * (coupon.Value / 100m),
            DiscountType.Flat => coupon.Value,
            _ => 0,
        };

        if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
            discountAmount = coupon.MaxDiscountAmount.Value;

        if (discountAmount > subTotal)
            discountAmount = subTotal;

        var result = new DiscountResult(coupon.Code, discountAmount, subTotal - discountAmount);
        return Result<DiscountResult>.Success(result);
    }

    public async Task<Result<DiscountResult>> ApplyScholarshipAsync(decimal subTotal, Guid scholarshipId, CancellationToken cancellationToken)
    {
        // Placeholder: would fetch scholarship from repository
        var discountAmount = subTotal * 0.25m;
        var result = new DiscountResult("Scholarship", discountAmount, subTotal - discountAmount);
        return Result<DiscountResult>.Success(result);
    }

    public async Task<Result<DiscountResult>> ApplyDiscountPolicyAsync(decimal subTotal, Guid policyId, CancellationToken cancellationToken)
    {
        // Placeholder: would fetch discount policy from repository
        var discountAmount = subTotal * 0.10m;
        var result = new DiscountResult("Discount Policy", discountAmount, subTotal - discountAmount);
        return Result<DiscountResult>.Success(result);
    }
}
