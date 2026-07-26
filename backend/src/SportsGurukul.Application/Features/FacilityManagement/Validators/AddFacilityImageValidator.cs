using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.AddFacilityImage;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class AddFacilityImageValidator : AbstractValidator<AddFacilityImageCommand>
{
    public AddFacilityImageValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(2000).WithMessage("Image URL must not exceed 2000 characters.");
    }
}
