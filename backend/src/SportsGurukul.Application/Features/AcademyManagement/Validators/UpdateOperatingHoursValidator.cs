using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateOperatingHours;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class UpdateOperatingHoursValidator : AbstractValidator<UpdateOperatingHoursCommand>
{
    private const string TimePattern = @"^([01]\d|2[0-3]):[0-5]\d$";

    public UpdateOperatingHoursValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.MondayOpening)
            .Matches(TimePattern).WithMessage("Opening time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MondayOpening));

        RuleFor(x => x.MondayClosing)
            .Matches(TimePattern).WithMessage("Closing time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MondayClosing));

        RuleFor(x => x.TuesdayOpening)
            .Matches(TimePattern).WithMessage("Opening time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TuesdayOpening));

        RuleFor(x => x.TuesdayClosing)
            .Matches(TimePattern).WithMessage("Closing time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TuesdayClosing));

        RuleFor(x => x.WednesdayOpening)
            .Matches(TimePattern).WithMessage("Opening time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.WednesdayOpening));

        RuleFor(x => x.WednesdayClosing)
            .Matches(TimePattern).WithMessage("Closing time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.WednesdayClosing));

        RuleFor(x => x.ThursdayOpening)
            .Matches(TimePattern).WithMessage("Opening time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.ThursdayOpening));

        RuleFor(x => x.ThursdayClosing)
            .Matches(TimePattern).WithMessage("Closing time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.ThursdayClosing));

        RuleFor(x => x.FridayOpening)
            .Matches(TimePattern).WithMessage("Opening time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FridayOpening));

        RuleFor(x => x.FridayClosing)
            .Matches(TimePattern).WithMessage("Closing time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FridayClosing));

        RuleFor(x => x.SaturdayOpening)
            .Matches(TimePattern).WithMessage("Opening time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SaturdayOpening));

        RuleFor(x => x.SaturdayClosing)
            .Matches(TimePattern).WithMessage("Closing time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SaturdayClosing));

        RuleFor(x => x.SundayOpening)
            .Matches(TimePattern).WithMessage("Opening time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SundayOpening));

        RuleFor(x => x.SundayClosing)
            .Matches(TimePattern).WithMessage("Closing time must be in HH:mm format.")
            .MaximumLength(5).WithMessage("Time must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SundayClosing));
    }
}
