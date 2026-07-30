using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepository;

    public CouponService(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Result<CouponDto>> CreateCouponAsync(CreateCouponRequest request, CancellationToken cancellationToken)
    {
        var existing = await _couponRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
            return Result<CouponDto>.Failure("Coupon code already exists");

        var coupon = new Coupon
        {
            Code = request.Code.ToUpperInvariant(),
            Type = request.DiscountType,
            Value = request.Value,
            MinOrderAmount = request.MinimumOrderAmount,
            MaxDiscountAmount = request.MaximumDiscountAmount,
            MaxUsage = request.MaxUsages,
            CurrentUsage = 0,
            ValidFrom = request.ValidFrom ?? DateTime.UtcNow,
            ValidTo = request.ValidTo ?? DateTime.UtcNow.AddDays(30),
            IsActive = true,
        };

        var created = await _couponRepository.AddAsync(coupon, cancellationToken);
        return Result<CouponDto>.Success(MapToDto(created));
    }

    public async Task<Result<CouponDto>> UpdateCouponAsync(Guid couponId, UpdateCouponRequest request, CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByIdAsync(couponId, cancellationToken);
        if (coupon is null)
            return Result<CouponDto>.Failure("Coupon not found");

        if (request.Value.HasValue) coupon.Value = request.Value.Value;
        if (request.MinimumOrderAmount.HasValue) coupon.MinOrderAmount = request.MinimumOrderAmount;
        if (request.MaximumDiscountAmount.HasValue) coupon.MaxDiscountAmount = request.MaximumDiscountAmount;
        if (request.MaxUsages.HasValue) coupon.MaxUsage = request.MaxUsages;
        if (request.ValidFrom.HasValue) coupon.ValidFrom = request.ValidFrom.Value;
        if (request.ValidTo.HasValue) coupon.ValidTo = request.ValidTo.Value;

        _couponRepository.Update(coupon);
        return Result<CouponDto>.Success(MapToDto(coupon));
    }

    public async Task<Result<CouponDto>> ApplyCouponAsync(string code, string? userId, decimal orderAmount, CancellationToken cancellationToken)
    {
        var validation = await ValidateCouponAsync(code, userId, orderAmount, cancellationToken);
        if (!validation.IsSuccess || !validation.Value)
            return Result<CouponDto>.Failure(validation.Error ?? "Coupon validation failed");

        var coupon = await _couponRepository.GetByCodeWithUsagesAsync(code, cancellationToken);
        if (coupon is null)
            return Result<CouponDto>.Failure("Coupon not found");

        coupon.CurrentUsage++;
        _couponRepository.Update(coupon);

        return Result<CouponDto>.Success(MapToDto(coupon));
    }

    public async Task<Result<CouponDto>> ExpireCouponAsync(Guid couponId, CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByIdAsync(couponId, cancellationToken);
        if (coupon is null)
            return Result<CouponDto>.Failure("Coupon not found");

        coupon.IsActive = false;
        _couponRepository.Update(coupon);

        return Result<CouponDto>.Success(MapToDto(coupon));
    }

    public async Task<Result<bool>> ValidateCouponAsync(string code, string? userId, decimal orderAmount, CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByCodeAsync(code, cancellationToken);
        if (coupon is null)
            return Result<bool>.Failure("Coupon not found");

        if (!coupon.IsActive)
            return Result<bool>.Failure("Coupon is not active");

        if (coupon.ValidFrom > DateTime.UtcNow)
            return Result<bool>.Failure("Coupon is not yet valid");

        if (coupon.ValidTo < DateTime.UtcNow)
            return Result<bool>.Failure("Coupon has expired");

        if (coupon.MaxUsage.HasValue && coupon.CurrentUsage >= coupon.MaxUsage.Value)
            return Result<bool>.Failure("Coupon usage limit exceeded");

        if (coupon.MinOrderAmount.HasValue && orderAmount < coupon.MinOrderAmount.Value)
            return Result<bool>.Failure($"Minimum order amount of {coupon.MinOrderAmount.Value} not met");

        return Result<bool>.Success(true);
    }

    public async Task<Result<CouponDto>> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByCodeAsync(code, cancellationToken);
        if (coupon is null)
            return Result<CouponDto>.Failure("Coupon not found");

        return Result<CouponDto>.Success(MapToDto(coupon));
    }

    private static CouponDto MapToDto(Coupon coupon)
    {
        return new CouponDto(
            coupon.Id,
            coupon.Code,
            null,
            coupon.Type,
            coupon.Value,
            coupon.MinOrderAmount,
            coupon.MaxDiscountAmount,
            coupon.MaxUsage,
            coupon.CurrentUsage,
            coupon.ValidFrom,
            coupon.ValidTo,
            coupon.IsActive,
            coupon.CreatedAt
        );
    }
}
