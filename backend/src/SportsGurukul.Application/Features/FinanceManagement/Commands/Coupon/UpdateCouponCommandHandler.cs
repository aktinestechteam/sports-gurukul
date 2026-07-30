using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, Result<CouponDto>>
{
    private readonly ICouponService _couponService;

    public UpdateCouponCommandHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<CouponDto>> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateCouponRequest(
            request.Description,
            request.Value,
            request.MinimumOrderAmount,
            request.MaximumDiscountAmount,
            request.MaxUsages,
            request.ValidFrom,
            request.ValidTo
        );
        return await _couponService.UpdateCouponAsync(request.CouponId, updateRequest, cancellationToken);
    }
}
