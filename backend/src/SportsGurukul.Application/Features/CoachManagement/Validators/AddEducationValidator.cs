using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddEducation;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class AddEducationValidator : AbstractValidator<AddEducationCommand>
{
    public AddEducationValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.Degree)
            .NotEmpty().WithMessage("Degree is required.")
            .MaximumLength(200).WithMessage("Degree must not exceed 200 characters.");

        RuleFor(x => x.Institution)
            .MaximumLength(300).WithMessage("Institution must not exceed 300 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Institution));

        RuleFor(x => x.FieldOfStudy)
            .MaximumLength(200).WithMessage("Field of study must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FieldOfStudy));

        RuleFor(x => x.YearCompleted)
            .InclusiveBetween(1950, 2100).WithMessage("Year completed must be between 1950 and 2100.")
            .When(x => x.YearCompleted.HasValue);
    }
}
