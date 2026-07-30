using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class ValidateCouponQueryHandler : IRequestHandler<ValidateCouponQuery, Result<bool>>
{
    private readonly ICouponService _couponService;

    public ValidateCouponQueryHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<bool>> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
    {
        return await _couponService.ValidateCouponAsync(request.Code, request.UserId, request.OrderAmount, cancellationToken);
    }
}
