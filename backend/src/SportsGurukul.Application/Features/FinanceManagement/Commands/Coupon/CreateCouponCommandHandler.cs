using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Result<CouponDto>>
{
    private readonly ICouponService _couponService;

    public CreateCouponCommandHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<CouponDto>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreateCouponRequest(
            request.Code,
            request.Description,
            request.DiscountType,
            request.Value,
            request.MinimumOrderAmount,
            request.MaximumDiscountAmount,
            request.MaxUsages,
            request.ValidFrom,
            request.ValidTo
        );
        return await _couponService.CreateCouponAsync(createRequest, cancellationToken);
    }
}
