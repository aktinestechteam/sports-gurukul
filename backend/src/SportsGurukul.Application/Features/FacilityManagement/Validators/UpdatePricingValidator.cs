using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.UpdatePricing;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class UpdatePricingValidator : AbstractValidator<UpdatePricingCommand>
{
    public UpdatePricingValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");

        RuleFor(x => x.PricingName)
            .NotEmpty().WithMessage("Pricing name is required.")
            .MaximumLength(100).WithMessage("Pricing name must not exceed 100 characters.");

        RuleFor(x => x.HourlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Hourly rate must be greater than or equal to 0.");

        RuleFor(x => x.DailyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Daily rate must be greater than or equal to 0.");

        RuleFor(x => x.MonthlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly rate must be greater than or equal to 0.");

        RuleFor(x => x.PeakHourlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Peak hourly rate must be greater than or equal to 0.")
            .When(x => x.PeakHourlyRate.HasValue);

        RuleFor(x => x.OffPeakHourlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Off-peak hourly rate must be greater than or equal to 0.")
            .When(x => x.OffPeakHourlyRate.HasValue);
    }
}
