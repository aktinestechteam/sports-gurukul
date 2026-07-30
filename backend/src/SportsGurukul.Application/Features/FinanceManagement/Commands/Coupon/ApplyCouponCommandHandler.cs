using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, Result<CouponDto>>
{
    private readonly ICouponService _couponService;

    public ApplyCouponCommandHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<CouponDto>> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
    {
        return await _couponService.ApplyCouponAsync(request.Code, request.UserId, request.OrderAmount, cancellationToken);
    }
}
