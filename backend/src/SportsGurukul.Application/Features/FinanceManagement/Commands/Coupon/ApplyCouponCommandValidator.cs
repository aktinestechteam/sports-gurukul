using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public class ApplyCouponCommandValidator : AbstractValidator<ApplyCouponCommand>
{
    public ApplyCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Coupon code is required");
        RuleFor(x => x.OrderAmount).GreaterThan(0).WithMessage("Order amount must be greater than zero");
    }
}
