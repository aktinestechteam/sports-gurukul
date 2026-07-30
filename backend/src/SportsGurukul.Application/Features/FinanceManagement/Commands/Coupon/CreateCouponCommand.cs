using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public record CreateCouponCommand(string Code, string? Description, DiscountType DiscountType, decimal Value, decimal? MinimumOrderAmount, decimal? MaximumDiscountAmount, int? MaxUsages, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<Result<CouponDto>>;
