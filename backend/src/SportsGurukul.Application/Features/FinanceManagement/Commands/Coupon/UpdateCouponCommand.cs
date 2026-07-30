using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public record UpdateCouponCommand(Guid CouponId, string? Description, decimal? Value, decimal? MinimumOrderAmount, decimal? MaximumDiscountAmount, int? MaxUsages, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<Result<CouponDto>>;
