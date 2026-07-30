using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, Result<IReadOnlyList<CouponDto>>>
{
    private readonly ICouponRepository _couponRepository;

    public GetAllCouponsQueryHandler(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Result<IReadOnlyList<CouponDto>>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
    {
        var coupons = await _couponRepository.GetAllAsync(cancellationToken);
        var dtos = coupons.Select(c => new CouponDto(
            c.Id, c.Code, null, c.Type, c.Value, c.MinOrderAmount, c.MaxDiscountAmount,
            c.MaxUsage, c.CurrentUsage, c.ValidFrom, c.ValidTo, c.IsActive, c.CreatedAt
        )).ToList();
        return Result<IReadOnlyList<CouponDto>>.Success(dtos);
    }
}
