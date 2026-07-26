using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateSocialLinks;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class UpdateSocialLinksValidator : AbstractValidator<UpdateSocialLinksCommand>
{
    public UpdateSocialLinksValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.Links)
            .NotEmpty().WithMessage("At least one social link is required.");

        RuleForEach(x => x.Links).ChildRules(link =>
        {
            link.RuleFor(l => l.Platform)
                .NotEmpty().WithMessage("Platform name is required.")
                .MaximumLength(50).WithMessage("Platform name must not exceed 50 characters.");

            link.RuleFor(l => l.Url)
                .NotEmpty().WithMessage("URL is required.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("A valid URL is required.")
                .MaximumLength(500).WithMessage("URL must not exceed 500 characters.");
        });
    }
}
