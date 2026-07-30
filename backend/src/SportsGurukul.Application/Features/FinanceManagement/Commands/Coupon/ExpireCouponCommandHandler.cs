using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public class ExpireCouponCommandHandler : IRequestHandler<ExpireCouponCommand, Result<CouponDto>>
{
    private readonly ICouponService _couponService;

    public ExpireCouponCommandHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<CouponDto>> Handle(ExpireCouponCommand request, CancellationToken cancellationToken)
    {
        return await _couponService.ExpireCouponAsync(request.CouponId, cancellationToken);
    }
}
