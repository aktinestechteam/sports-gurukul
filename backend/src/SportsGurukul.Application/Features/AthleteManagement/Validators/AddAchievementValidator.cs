using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AddAchievement;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class AddAchievementValidator : AbstractValidator<AddAchievementCommand>
{
    public AddAchievementValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Competition)
            .MaximumLength(200).WithMessage("Competition must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Competition));

        RuleFor(x => x.Position)
            .MaximumLength(100).WithMessage("Position must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Position));

        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future.");

        RuleFor(x => x.CertificateUrl)
            .MaximumLength(2000).WithMessage("Certificate URL must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.CertificateUrl));

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
