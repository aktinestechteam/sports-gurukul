using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public record ApplyCouponCommand(string Code, string? UserId, decimal OrderAmount) : IRequest<Result<CouponDto>>;
