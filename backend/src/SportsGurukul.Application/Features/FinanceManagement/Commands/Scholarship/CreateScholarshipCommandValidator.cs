using FluentValidation;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;

public class CreateScholarshipCommandValidator : AbstractValidator<CreateScholarshipCommand>
{
    public CreateScholarshipCommandValidator()
    {
        RuleFor(x => x.AthleteId).NotEmpty().WithMessage("Athlete is required");
        RuleFor(x => x.Value).GreaterThan(0).WithMessage("Value must be greater than zero");
        RuleFor(x => x.DiscountType).IsInEnum().WithMessage("Invalid discount type");
        RuleFor(x => x.ValidTo).GreaterThan(x => x.ValidFrom).When(x => x.ValidFrom.HasValue && x.ValidTo.HasValue).WithMessage("ValidTo must be after ValidFrom");
    }
}
