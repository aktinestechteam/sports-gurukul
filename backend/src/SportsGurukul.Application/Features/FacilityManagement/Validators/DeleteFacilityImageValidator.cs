using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteFacilityImage;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class DeleteFacilityImageValidator : AbstractValidator<DeleteFacilityImageCommand>
{
    public DeleteFacilityImageValidator()
    {
        RuleFor(x => x.ImageId)
            .NotEmpty().WithMessage("Image ID is required.");
    }
}
