using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateBranch;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class UpdateBranchValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchValidator()
    {
        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch ID is required.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.BranchName)
            .MaximumLength(200).WithMessage("Branch name must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.BranchName));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Country));

        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.State));

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.District)
            .MaximumLength(100).WithMessage("District must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.District));

        RuleFor(x => x.PostalCode)
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.")
            .When(x => x.Longitude.HasValue);
    }
}
