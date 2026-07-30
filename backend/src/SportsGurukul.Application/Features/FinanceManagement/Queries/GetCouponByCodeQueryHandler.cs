using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetCouponByCodeQueryHandler : IRequestHandler<GetCouponByCodeQuery, Result<CouponDto>>
{
    private readonly ICouponService _couponService;

    public GetCouponByCodeQueryHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<CouponDto>> Handle(GetCouponByCodeQuery request, CancellationToken cancellationToken)
    {
        return await _couponService.GetByCodeAsync(request.Code, cancellationToken);
    }
}
