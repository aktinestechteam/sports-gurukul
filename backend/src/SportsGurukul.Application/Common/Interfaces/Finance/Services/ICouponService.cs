using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface ICouponService
{
    Task<Result<CouponDto>> CreateCouponAsync(CreateCouponRequest request, CancellationToken cancellationToken = default);
    Task<Result<CouponDto>> UpdateCouponAsync(Guid couponId, UpdateCouponRequest request, CancellationToken cancellationToken = default);
    Task<Result<CouponDto>> ApplyCouponAsync(string code, string? userId, decimal orderAmount, CancellationToken cancellationToken = default);
    Task<Result<CouponDto>> ExpireCouponAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateCouponAsync(string code, string? userId, decimal orderAmount, CancellationToken cancellationToken = default);
    Task<Result<CouponDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
