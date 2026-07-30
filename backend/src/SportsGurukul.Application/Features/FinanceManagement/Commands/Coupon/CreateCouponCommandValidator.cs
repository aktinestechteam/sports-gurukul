using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50).WithMessage("Coupon code is required and must be at most 50 characters");
        RuleFor(x => x.Value).GreaterThan(0).WithMessage("Value must be greater than zero");
        RuleFor(x => x.DiscountType).IsInEnum().WithMessage("Invalid discount type");
        RuleFor(x => x.ValidTo).GreaterThan(x => x.ValidFrom).When(x => x.ValidFrom.HasValue && x.ValidTo.HasValue).WithMessage("ValidTo must be after ValidFrom");
    }
}
